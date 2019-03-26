import {BaseController} from "../core/BaseController";
import {LoginResponse, LoginState} from "../core/Responses/LoginResponse";
import {ObjectId} from "bson";
import {StatusCodes} from "../core/StatusCodes";
import {userInfo} from "os";
import {identifier} from "babel-types";
const hasher = require('../core/Hasher');
const jwt = require('jsonwebtoken');
const randomstring = require('randomstring');
const schedule = require('node-schedule');
const { check, validationResult } = require('express-validator/check');

export class UsersController extends BaseController {
    public registerRoutes() {
        super.registerRoutes();

        var that = this;
        var sch = schedule.scheduleJob('0 * * ? * *', function () {

            that.getDb().collection('users').find({state: "Unapproved"}).toArray(function (err, result) {

                if(err) {
                    console.log('Error while trying to clear database from "Unapproved" registration(s)!');
                    return;
                }

                console.log('Checking for deletable, "Unapproved" registration(s).');

                result.forEach(function (element) {
                    if(element._id.generationTime < Math.floor(new Date().getTime() / 1000) - 3 * 60) {
                        console.log('Deleting user with the following id: ', element._id);
                        that.getDb().collection('users').deleteOne({_id: element._id});
                    }
                });
            });

        });

        this.router.route('/login')
            .post(async (req, res) => {
                console.log(req.get('Authorization'));
                const email: string = req.get('email');
                const password: string = req.get('password');
                if(email == null || password == null){
                    res.status(StatusCodes.InternalError).send();
                    return;
                }

                const hashedPassword: string = hasher(password);

                await this.getDb().collection('users').findOne(
                    { $and: [
                            {email: email},
                            {password: hashedPassword},
                            {state: "Approved"}
                        ] },async (err, user) => {
                    // In case the user not found
                    if(err) {
                        res.status(StatusCodes.InternalError).send(err);
                        return;
                    }
                    if (user){
                        // TODO: move secret to config file
                        const token: string = jwt.sign({userId: user._id}, 'shhhhhhhhhhhhhh');
                        res.status(StatusCodes.OK).send(new LoginResponse(
                            LoginState.OK,
                            token)
                        );
                    } else {
                        res.status(StatusCodes.Unauthorized).send(new LoginResponse(
                            LoginState.BadAuth,
                            '')
                        );
                    }
                });
            });

        this.router.route('/validateemail/:token')
            .post(async (req, res) =>{
                const token: string = req.params.token;

                await this.getDb().collection('users').updateOne(
                    {$and: [
                            {token: token},
                            {state: 'Unapproved'}
                        ]},
                    {
                            $unset: { token: '' },
                            $set: {state: 'Approved'}
                        },
                    async (err, user) =>{
                        // In case the user not found
                        if(err) {
                            res.status(StatusCodes.InternalError).send(err);
                            return;
                        }
                        if (user && user.modifiedCount === 1){
                            res.status(StatusCodes.OK).send();
                        } else {
                            res.status(StatusCodes.Forbidden).send();
                        }
                    }
                );
            });

        this.router.get('/listgroups', async (req, res) => {
            const userId: ObjectId = await this.authenticator.check(req.get('Authorization'));

            if(userId == null) {
                res.status(StatusCodes.Forbidden).send("");
                return;
            }

            await this.getDb().collection("users").findOne(
                {_id:userId},
                {fields:{_id:false,groups:true}},
                async (error, result) => {
                    if(error) {
                        res.status(StatusCodes.InternalError).send(error);
                        return;
                    }

                    if(result){
                        if(result.groups != null)
                            res.status(StatusCodes.OK).send(result.groups);
                        else
                            res.status(StatusCodes.OK).send(new Array<ObjectId>());
                    }
                    else{
                        res.status(StatusCodes.InternalError).send(result);
                    }
                }
            );
        });

        this.router.post('/updategroups', async (req, res) => {
            const userId = await this.authenticator.check(req.get('Authorization'));

            if(userId == null) {
                res.status(StatusCodes.Forbidden).send("");
                return;
            }

            let documents = await this.getDb().collection("groups").find(
                {creator:userId},
                {fields:{_id:true}}
                ).toArray();

            let groupIds: Array<ObjectId> = documents.map(function (document):ObjectId {
                return document._id;
            });

            await this.getDb().collection("users").updateOne(
                {_id:userId},
                {$set: {groups:groupIds}},
                async (error, result) => {
                    if(error) {
                        res.status(StatusCodes.InternalError).send(error);
                        return;
                    }

                    if(result){
                        res.status(StatusCodes.OK).send(result);
                    }
                    else{
                        res.status(StatusCodes.InternalError).send(result);
                    }
                }
            );
        });

        this.router.get('/listUsers', async (req, res) => {
            let docs = await this.getDb().collection("users").find().toArray();

            console.log(docs);
            res.send(docs);
        });

        this.router.post('/createUser',[
            check('email').isEmail(),
            check('password').matches(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{8,}$/, "i")
        ], async (req, res) => {

            const errors = validationResult(req);
            if (!errors.isEmpty()) {
                return res.status(StatusCodes.Unprocessable).json({ errors: errors.array() });
            }

            var password = req.body.password;
            var token = hasher( req.body.email + new Date().getTime() + randomstring.generate());
            const hashedPassword: string = hasher(password);

            await this.getDb().collection("users").updateOne(
                {email : req.body.email},
                { $setOnInsert: {
                        email: req.body.email,
                        password: hashedPassword,
                        displayname: req.body.displayname,
                        state: "Unapproved",
                        groups: [],
                        token: token
                    }
                },
                {upsert : true},
                async (error, result) => {

                    if(result.matchedCount !== 0) {
                        res.status(StatusCodes.Conflict).send("Duplicated registration.");
                        return;
                    }

                    if(error) {
                        res.status(StatusCodes.InternalError).send(error);
                        return;
                    }

                    if(result && result.matchedCount  === 0){
                        res.status(StatusCodes.OKCreated).send(result);

                        this.getEmail().sendMailHtml(req.body.email, "Regisztráció megerősítése", "A megerősítéshez kattintson <a href='" + this.config.site_url_for_user + "account/confirm/" + token + "'>IDE</a>");
                    }
                    else{
                        res.status(StatusCodes.InternalError).send(result);
                        return;
                    }
                }
            );
        })


    }
}

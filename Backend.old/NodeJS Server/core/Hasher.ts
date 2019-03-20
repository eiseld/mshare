const sha1 = require('node-sha1');

export = function(value: string) {
    // TODO: Add salt
    return sha1(value).toLowerCase();
}

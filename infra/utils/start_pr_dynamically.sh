#!/bin/sh
git fetch origin +refs/pull/${pr_name}/merge
git checkout FETCH_HEAD
sqlite3 ${db} 'create table if not exists pr (id INTEGER, instance_count INTEGER, pr TEXT)'
count=$(sqlite3 ${db} 'select count(id) from pr')
if [ "$count" -eq "0" ]; then
    sqlite3 ${db} "insert into pr (id,instance_count) VALUES (0,0),(1,0),(2,0),(3,0),(4,0),(5,0),(6,0),(7,0),(8,0),(9,0)"
fi
first=$(sqlite3 ${db} "select id from pr where pr = ${pr_name} LIMIT 1")
if [ -z "$first" ]; then
	first=$(sqlite3 ${db} 'select id from pr where instance_count = 0 LIMIT 1')
fi
sqlite3 ${db} "update pr set instance_count = 1, pr = '${pr_name}' where id = ${first}"
sudo sed -i "s/pr_aspnet:/pr_aspnet_${pr_name}:/g" infra/docker/docker-compose-pr.yml 
sudo sed -i "s/pr_proxy:/pr_proxy_${pr_name}:/g" infra/docker/docker-compose-pr.yml 
(export pr_port=809${first} pr_version=${first}; cd ./infra/docker/; sudo -E docker-compose -f docker-compose-pr.yml build; sudo -E docker-compose -f docker-compose-pr.yml up -d)
echo $first
sed -i "s/test\/api/pr_${first}\/api/g" frontend/android/app/build.gradle 
cd ./frontend/android/
chmod +x gradlew
./gradlew assembleDebug
sudo cp ${WORKSPACE}/frontend/android/app/build/outputs/apk/staging/debug/app-staging-debug.apk /var/www/html/file/android/mshare-staging-${first}.apk
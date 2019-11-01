#!/bin/sh
db="${1:-jenkins.db}"
sqlite3 ${db} 'create table if not exists pr (id INTEGER, instance_count INTEGER)'
count=$(sqlite3 ${db} 'select count(id) from pr')
if [ "$count" -eq "0" ]; then
    sqlite3 ${db} "insert into pr (id,instance_count) VALUES (0,0),(1,0),(2,0),(3,0),(4,0),(5,0),(6,0),(7,0),(8,0),(9,0)"
    count=$(sqlite3 ${db} 'select count(id) from pr')
fi
first=$(sqlite3 ${db} 'select id from pr where instance_count = 0 LIMIT 1')
sqlite3 ${db} "update pr set instance_count = 1 where id = ${first}"
(export pr_port=809${first} pr_version=${first}; cd ../docker/; sudo -E docker-compose -f docker-compose-pr.yml build; sudo -E docker-compose -f docker-compose-pr.yml up -d)
echo $first
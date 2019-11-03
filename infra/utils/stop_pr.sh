#!/bin/sh
git fetch origin +refs/pull/${pr_name}/merge
git checkout FETCH_HEAD
n=$(sqlite3 ${db} "select id from pr where pr = '${pr_name}' LIMIT 1")
(export pr_port=809${n} pr_version=${n}; cd ./infra/docker/; sudo -E docker-compose -f docker-compose-pr.yml down)
sqlite3 ${db} "update pr set instance_count = 0, pr = '' where id = ${n}"
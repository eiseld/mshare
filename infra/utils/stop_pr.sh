#!/bin/sh
db="${1:-jenkins.db}"
n="${2:-0}"
(export pr_port=809${n} pr_version=${n}; cd ../docker/; sudo -E docker-compose -f docker-compose-pr.yml down; sudo -E docker-compose -f docker-compose-pr.yml remove)
sqlite3 ${db) "update pr set instance_count = 0 where id = ${n}"
n="${1:-0}"
(export pr_port=809${n} pr_version=${n}; cd ../docker/; sudo -E docker-compose -f docker-compose-pr.yml build; sudo -E docker-compose -f docker-compose-pr.yml up -d)
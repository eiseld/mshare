:: Delete database
docker exec -it mshare-mysql bash -c "mysql --user=root --password=ilovescrum --execute=\"drop DATABASE if exists mshare;CREATE DATABASE mshare;\""

:: Import structure files
for %%x in (email_types users groups users_groups_map email_tokens spendings debtors settlements optimized_debt test history dbevents) do docker exec -i mshare-mysql mysql --user=root --password=ilovescrum mshare < ./dump/%%x.sql

:: Copy data files to docker
docker cp ./dump mshare-mysql:/

:: Import data files
for %%x in (email_types users groups users_groups_map email_tokens spendings debtors settlements optimized_debt test history) do docker exec -i mshare-mysql mysqlimport --user=root --password=ilovescrum --ignore --force --fields-terminated-by="," --lines-terminated-by="\r\n" mshare /dump/%%x.txt
PAUSE
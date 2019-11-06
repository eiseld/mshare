rmdir /s /q dump
mkdir dump

docker exec mshare-mysql rm -r /dump
docker exec mshare-mysql mkdir -p -m 777 dump

docker exec -i mshare-mysql mysqldump -u root --events --routines --triggers --tab="/dump/" mshare > ./dump/dbevents.sql

docker cp mshare-mysql:/dump .
PAUSE
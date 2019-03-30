1, Mysql
	version: 8.0.15
	internal address: 'mshare-mysql:3306'
	exrernal address: '<docker ip>:3306'
	database name: 'mshare'
	root password: 'ilovescrum'
	
2, Aspnet
	version: 2.2
	internal address: 'mshare-aspnet:8081'
	exrernal address: '<docker ip>:8081'
	ASPNETCORE_ENVIRONMENT:
	  'Development' //In prod 
	  'Production' //In aspnet, angular and fullstack
	MSHARE_RUNNING_BEHIND_PROXY: 'true'
	
3, Apache
	version: 2.4
	internal address: 'mshare-apache:80'
	exrernal address: '<docker ip>:80'
	config file: 
	  'httpd.conf.prod' //Used in prod
	  'httpd.conf.dev' //Used in aspnet
	  'httpd.conf.angular' //Used in fullstack and angular
	  
4, Angular
	node version: 11.12.0
	internal address: 'mshare-angular:4200'
	exrernal address: '<docker ip>:4200'
	
5, Startup order
  mysql => aspnet => apache
				  => angular
				  
6, Volumes
	Any of the dev composes require the sharing of some of the following folders:
		/frontend
		/backend

7, Start docker
	You can use the batch files provided in the /quickstart folder or
	you can use the following commands:
		prod: "docker-compose -f docker-compose-prod.yml up --build"
		angular-dev: "docker-compose -f docker-compose-prod.yml -f docker-compose-dev-angular.yml up --build"
		aspnet-dev: "docker-compose -f docker-compose-prod.yml -f docker-compose-dev-aspnet.yml up --build"
		fullstack-dev: "docker-compose -f docker-compose-prod.yml -f docker-compose-dev-fullstack.yml up --build"

8, Database
	You can use "MYSQL Workbench 8.0 CE" to connect to the database.
	Hostname: <docker-ip>
	Port: 3306
	Username: 'root'
	Password: 'ilovescrum'

	To quickly import or export the database state use the batch files provided in the
	/backend/database folder or use the following commands:
		import: "docker exec -i mshare-mysql mysql --user=root --password=ilovescrum mshare < data.sql"
		export: "docker exec -i mshare-mysql mysqldump --user=root --password=ilovescrum mshare > data.sql"
		
	!!!Warning: data.sql file contains test data, do not push to repository if you changed it after testing!!!

8, Good to know
	Aspnet live reload works, but you cannot debug in it.
	Angular live reload keeps a separate /node_modules folder in the container and ignores the local one.
	Angular live reload generate the contents of /node_modules folder based on the package.json file when you start the docker.

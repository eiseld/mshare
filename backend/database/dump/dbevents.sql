
--
-- Dumping events for database 'mshare'
--
/*!50106 SET @save_time_zone= @@TIME_ZONE */ ;
/*!50106 DROP EVENT IF EXISTS `clean_registrations` */;
DELIMITER ;;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;;
/*!50003 SET character_set_client  = utf8mb4 */ ;;
/*!50003 SET character_set_results = utf8mb4 */ ;;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;;
/*!50003 SET @saved_time_zone      = @@time_zone */ ;;
/*!50003 SET time_zone             = 'SYSTEM' */ ;;
/*!50106 CREATE*/ /*!50117 DEFINER=`root`@`%`*/ /*!50106 EVENT `clean_registrations` ON SCHEDULE EVERY 1 MINUTE STARTS '2019-03-31 10:13:25' ON COMPLETION NOT PRESERVE ENABLE COMMENT 'Clears out non validated registrations and expired password mails every 5 minutes!' DO BEGIN
			DECLARE validate_email INTEGER;
			DECLARE forgotten_psw INTEGER;
            
			SET validate_email = (SELECT id FROM `mshare`.`email_types` WHERE type_name = "validate_email");
			SET forgotten_psw = (SELECT id FROM `mshare`.`email_types` WHERE type_name = "forgotten_psw");
			DELETE FROM `mshare`.`users` WHERE id IN 
				(SELECT user_id FROM `mshare`.`email_tokens`
					WHERE expiration_date IS NOT NULL
						AND expiration_date <= now()
						AND token_type = validate_email);
                        
			DELETE FROM `mshare`.`email_tokens`
				WHERE token_type = forgotten_psw
					AND expiration_date IS NOT NULL
					AND expiration_date <= now();
		END */ ;;
/*!50003 SET time_zone             = @saved_time_zone */ ;;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;;
/*!50003 SET character_set_client  = @saved_cs_client */ ;;
/*!50003 SET character_set_results = @saved_cs_results */ ;;
/*!50003 SET collation_connection  = @saved_col_connection */ ;;
DELIMITER ;
/*!50106 SET TIME_ZONE= @save_time_zone */ ;

--
-- Dumping routines for database 'mshare'
--

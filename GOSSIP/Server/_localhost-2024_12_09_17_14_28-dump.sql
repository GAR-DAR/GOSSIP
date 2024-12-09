-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: gossip
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `chats`
--

DROP TABLE IF EXISTS `chats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chats` (
  `id` int NOT NULL AUTO_INCREMENT,
  `created_at` datetime NOT NULL,
  `name` varchar(255) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chats`
--

LOCK TABLES `chats` WRITE;
/*!40000 ALTER TABLE `chats` DISABLE KEYS */;
INSERT INTO `chats` VALUES (1,'2024-11-19 12:57:57','os',0),(2,'2024-11-19 13:00:27','huh',0),(3,'0001-01-01 00:00:00','Sectumsembra',0);
/*!40000 ALTER TABLE `chats` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chats_to_users`
--

DROP TABLE IF EXISTS `chats_to_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chats_to_users` (
  `chat_id` int NOT NULL,
  `user_id` int NOT NULL,
  KEY `user_id` (`user_id`),
  KEY `chat_id` (`chat_id`),
  CONSTRAINT `chats_to_users_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`),
  CONSTRAINT `chats_to_users_ibfk_2` FOREIGN KEY (`chat_id`) REFERENCES `chats` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chats_to_users`
--

LOCK TABLES `chats_to_users` WRITE;
/*!40000 ALTER TABLE `chats_to_users` DISABLE KEYS */;
INSERT INTO `chats_to_users` VALUES (1,1),(1,2),(1,3),(1,5),(1,6),(1,7),(1,8),(1,9),(2,5),(2,8),(3,7),(3,10),(2,5);
/*!40000 ALTER TABLE `chats_to_users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `degrees`
--

DROP TABLE IF EXISTS `degrees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `degrees` (
  `id` int NOT NULL AUTO_INCREMENT,
  `degree` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `degrees`
--

LOCK TABLES `degrees` WRITE;
/*!40000 ALTER TABLE `degrees` DISABLE KEYS */;
INSERT INTO `degrees` VALUES (1,'Undergraduate'),(2,'Bachelor'),(3,'Master'),(4,'Postgraduate'),(5,'Doctor of Philosophy'),(6,'Doctor of Arts'),(7,'Doctor of Sciences');
/*!40000 ALTER TABLE `degrees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fields_of_study`
--

DROP TABLE IF EXISTS `fields_of_study`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fields_of_study` (
  `id` int NOT NULL AUTO_INCREMENT,
  `field` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  UNIQUE KEY `field` (`field`)
) ENGINE=InnoDB AUTO_INCREMENT=89 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fields_of_study`
--

LOCK TABLES `fields_of_study` WRITE;
/*!40000 ALTER TABLE `fields_of_study` DISABLE KEYS */;
INSERT INTO `fields_of_study` VALUES (20,'Agricultural Sciences'),(19,'Architecture & Building'),(2,'Arts & Culture'),(9,'Biology'),(15,'Cat Sciences'),(16,'Chemical Engineering & Bioengineering'),(26,'Civil Security'),(1,'Education'),(14,'Electrical Engineering'),(17,'Electronics & Automation'),(22,'Healthcare'),(3,'Human Sciences'),(12,'Information Technology'),(29,'International Relations'),(6,'Journalism'),(8,'Law'),(7,'Management & Administration'),(11,'Mathematics & Statistics'),(13,'Mechanical Engineering'),(25,'Military & Defence'),(10,'Natural Sciences'),(18,'Production & Technology'),(28,'Prompt Engineering'),(4,'Religion & Theology'),(24,'Service Sector'),(5,'Social Sciences'),(23,'Social Work'),(27,'Transport'),(21,'Veterinary');
/*!40000 ALTER TABLE `fields_of_study` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `messages`
--

DROP TABLE IF EXISTS `messages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `messages` (
  `id` int NOT NULL AUTO_INCREMENT,
  `chat_id` int NOT NULL,
  `sender_id` int NOT NULL,
  `content` mediumtext NOT NULL,
  `sent_at` datetime NOT NULL,
  `is_read` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  KEY `chat_id` (`chat_id`),
  KEY `sender_id` (`sender_id`),
  CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`chat_id`) REFERENCES `chats` (`id`),
  CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`sender_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `messages`
--

LOCK TABLES `messages` WRITE;
/*!40000 ALTER TABLE `messages` DISABLE KEYS */;
INSERT INTO `messages` VALUES (1,3,7,'Some message, isn\'t it?','2024-12-03 16:44:24',0,0),(2,1,3,'го на сидр','2024-12-09 16:46:12',1,0),(3,1,9,'го','2024-12-09 16:47:37',1,0),(4,1,7,'some message I\'ll delete','2024-12-09 16:51:38',1,1),(5,1,2,'еммммм..','2024-12-09 16:53:35',0,1),(6,1,6,'То треба їхати в Терник на сидри','2024-12-09 16:54:26',1,0),(7,1,8,'о, їдемо','2024-12-09 16:55:35',1,0),(8,1,1,'а далі в мене не вистачило уяви на тестові повідомлення','2024-12-09 16:57:03',1,0),(9,1,5,'тому','2024-12-09 16:57:37',1,0),(10,1,5,'вже як є','2024-12-09 16:57:53',1,0),(11,1,9,'Ну тоді біда)','2024-12-09 16:58:22',0,0),(12,2,7,'м, це не було моє питання','2024-12-09 17:14:00',0,0);
/*!40000 ALTER TABLE `messages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `replies`
--

DROP TABLE IF EXISTS `replies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `replies` (
  `id` int NOT NULL AUTO_INCREMENT,
  `creator_id` int NOT NULL,
  `topic_id` int NOT NULL,
  `parent_reply_id` int DEFAULT NULL,
  `reply_to` int DEFAULT NULL,
  `content` mediumtext NOT NULL,
  `created_at` datetime NOT NULL,
  `votes` int NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  KEY `topic_id` (`topic_id`),
  KEY `user_id` (`creator_id`),
  KEY `parent_reply_id` (`parent_reply_id`),
  KEY `replies_users_id_fk` (`reply_to`),
  CONSTRAINT `replies_ibfk_1` FOREIGN KEY (`topic_id`) REFERENCES `topics` (`id`),
  CONSTRAINT `replies_ibfk_2` FOREIGN KEY (`creator_id`) REFERENCES `users` (`id`),
  CONSTRAINT `replies_ibfk_3` FOREIGN KEY (`parent_reply_id`) REFERENCES `replies` (`id`),
  CONSTRAINT `replies_users_id_fk` FOREIGN KEY (`reply_to`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `replies`
--

LOCK TABLES `replies` WRITE;
/*!40000 ALTER TABLE `replies` DISABLE KEYS */;
INSERT INTO `replies` VALUES (1,3,3,NULL,NULL,'Ця Ірина...','2024-11-19 17:21:30',5,1),(2,1,1,NULL,NULL,'Windows','2024-11-19 17:24:31',100,0),(3,9,1,2,1,'Windows','2024-11-19 17:25:44',99,0),(4,8,10,NULL,NULL,'Накокетила','2024-12-07 18:13:28',0,0),(6,5,10,4,8,'Оууу єєє','2024-12-07 20:20:38',1,0),(7,9,10,4,5,'Хехе','2024-12-07 20:23:14',0,0),(8,9,10,4,5,'Хехе','2024-12-07 20:28:21',0,0);
/*!40000 ALTER TABLE `replies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `role` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'User'),(2,'Moderator');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `specializations`
--

DROP TABLE IF EXISTS `specializations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `specializations` (
  `id` int NOT NULL AUTO_INCREMENT,
  `specialization` varchar(255) NOT NULL,
  `official_id` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=128 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `specializations`
--

LOCK TABLES `specializations` WRITE;
/*!40000 ALTER TABLE `specializations` DISABLE KEYS */;
INSERT INTO `specializations` VALUES (1,'Educational and Pedagogical Sciences',11),(2,'Preschool Education',12),(3,'Primary Education',13),(4,'Secondary Education (by subject specializations)',14),(5,'Vocational Education (by specializations)',15),(6,'Special Education',16),(7,'Physical Culture and Sports',17),(8,'Audiovisual Arts and Production',21),(9,'Design',22),(10,'Fine Arts, Decorative Arts, Restoration',23),(11,'Choreography',24),(12,'Music Arts',25),(13,'Performing Arts',26),(14,'Museum Studies and Heritage Studies',27),(15,'Management of Socio-Cultural Activities',28),(16,'Information, Library, and Archival Studies',29),(17,'Religious Studies',31),(18,'History and Archaeology',32),(19,'Philosophy',33),(20,'Cultural Studies',34),(21,'Philology',35),(22,'Theology',41),(23,'Economics',51),(24,'Political Science',52),(25,'Psychology',53),(26,'Sociology',54),(27,'Journalism',61),(28,'Accounting and Taxation',71),(29,'Finance, Banking, and Insurance',72),(30,'Management',73),(31,'Marketing',75),(32,'Entrepreneurship, Trade, and Stock Exchange Activities',76),(33,'Law',81),(34,'Biology',91),(35,'Ecology',101),(36,'Chemistry',102),(37,'Earth Sciences',103),(38,'Physics and Astronomy',104),(39,'Applied Physics and Nanomaterials',105),(40,'Geography',106),(41,'Mathematics',111),(42,'Statistics',112),(43,'Applied Mathematics',113),(44,'Software Engineering',121),(45,'Computer Science',122),(46,'Computer Engineering',123),(47,'Systems Analysis',124),(48,'Cybersecurity',125),(49,'Information Systems and Technologies',126),(50,'Applied Mechanics',131),(51,'Materials Science',132),(52,'Mechanical Engineering',133),(53,'Aerospace Engineering',134),(54,'Shipbuilding',135),(55,'Metallurgy',136),(56,'Electrical Engineering and Electromechanics',141),(57,'Power Engineering',142),(58,'Nuclear Power',143),(59,'Thermal Power Engineering',144),(60,'Hydropower',145),(64,'Chemical Technology and Engineering',161),(65,'Biotechnology and Bioengineering',162),(66,'Biomedical Engineering',163),(67,'Electronics',171),(68,'Telecommunications and Radio Engineering',172),(69,'Avionics',173),(70,'Food Technologies',181),(71,'Light Industry Technologies',182),(72,'Environmental Protection Technologies',183),(73,'Mining',184),(74,'Oil and Gas Engineering and Technologies',185),(75,'Publishing and Printing',186),(76,'Woodworking and Furniture Technologies',187),(77,'Architecture and Urban Planning',191),(78,'Construction and Civil Engineering',192),(79,'Geodesy and Land Management',193),(80,'Hydraulic Engineering, Water Engineering, and Water Technologies',194),(81,'Agronomy',201),(82,'Plant Protection and Quarantine',202),(83,'Horticulture and Viticulture',203),(84,'Animal Production and Processing Technology',204),(85,'Forestry',205),(86,'Landscape Gardening',206),(87,'Aquatic Bioresources and Aquaculture',207),(88,'Agroengineering',208),(89,'Veterinary Medicine',211),(90,'Veterinary Hygiene, Sanitation, and Expertise',212),(91,'Dentistry',221),(92,'Medicine',222),(93,'Nursing',223),(94,'Medical Diagnostics and Treatment Technologies',224),(95,'Medical Psychology',225),(96,'Pharmacy, Industrial Pharmacy',226),(97,'Physical Therapy, Occupational Therapy',227),(98,'Pediatrics',228),(99,'Public Health',229),(100,'Social Work',231),(101,'Social Welfare',232),(102,'Hospitality and Restaurant Business',241),(103,'Tourism',242),(104,'State Security',251),(105,'Border Security',252),(106,'Military Administration',253),(107,'Troop Support',254),(108,'Armament and Military Equipment',255),(109,'National Security',256),(110,'Fire Safety',261),(111,'Law Enforcement Activities',262),(112,'Civil Security',263),(113,'River and Sea Transport',271),(114,'Aviation Transport',272),(115,'Rail Transport',273),(116,'Automobile Transport',274),(117,'Transport Technologies',275),(119,'International Relations, Public Communications, and Regional Studies',291),(120,'International Economic Relations',292),(121,'International Law',293),(122,'Frisky',151),(123,'Meow',152),(124,'ChatGPT',281),(125,'GitHub Copilot',282),(126,'Automation',174),(127,'Metrology',174);
/*!40000 ALTER TABLE `specializations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `statuses`
--

DROP TABLE IF EXISTS `statuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `statuses` (
  `id` int NOT NULL AUTO_INCREMENT,
  `status` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `statuses`
--

LOCK TABLES `statuses` WRITE;
/*!40000 ALTER TABLE `statuses` DISABLE KEYS */;
INSERT INTO `statuses` VALUES (1,'Student'),(2,'Faculty'),(3,'Learner'),(4,'None');
/*!40000 ALTER TABLE `statuses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `topics`
--

DROP TABLE IF EXISTS `topics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `topics` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `title` mediumtext NOT NULL,
  `content` mediumtext NOT NULL,
  `created_at` datetime NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `votes` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `topics_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `topics`
--

LOCK TABLES `topics` WRITE;
/*!40000 ALTER TABLE `topics` DISABLE KEYS */;
INSERT INTO `topics` VALUES (1,6,'Windows vs Linux','цей лінукс, бляха','2024-11-19 16:35:20',0,71),(2,9,'Я був на практичній з АК і можу багато чого розповісти','ну, це була невимовно класна практична, де Олег Григорович розповів нам компетентно про ігри та український геймдев','2024-11-19 16:48:50',0,69),(3,2,'АААААА ДАВАЙТЕ ДОДАМО ІМЕДЖІ ДО ЧАТУ','АААААААААААААААААААААААААААА','2024-11-19 16:53:11',0,-7),(8,10,'Майнкрафт то святе','хто не погодиться, тому перевірю конспект','2024-11-23 16:45:01',1,69),(9,10,'Майнкрафт то святе','Люблю майн і книжку з кдм','2024-11-25 12:31:19',0,21),(10,5,'Coquette','Накокетила','2024-12-05 13:41:37',0,69);
/*!40000 ALTER TABLE `topics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `topics_to_tags`
--

DROP TABLE IF EXISTS `topics_to_tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `topics_to_tags` (
  `id` int NOT NULL AUTO_INCREMENT,
  `topic_id` int NOT NULL,
  `tag` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `topics_to_tags_topics_id_fk` (`topic_id`),
  CONSTRAINT `topics_to_tags_topics_id_fk` FOREIGN KEY (`topic_id`) REFERENCES `topics` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `topics_to_tags`
--

LOCK TABLES `topics_to_tags` WRITE;
/*!40000 ALTER TABLE `topics_to_tags` DISABLE KEYS */;
INSERT INTO `topics_to_tags` VALUES (4,1,'OS'),(5,1,'Windows'),(6,8,'minecraft'),(7,8,'kdm'),(8,9,'minecraft'),(9,9,'kdm'),(10,9,'жура');
/*!40000 ALTER TABLE `topics_to_tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `universities`
--

DROP TABLE IF EXISTS `universities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `universities` (
  `id` int NOT NULL AUTO_INCREMENT,
  `university` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=249 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `universities`
--

LOCK TABLES `universities` WRITE;
/*!40000 ALTER TABLE `universities` DISABLE KEYS */;
INSERT INTO `universities` VALUES (1,'State University of Infrastructure and Technologies'),(2,'Cherkasy educational-scientific institute of the Banking University'),(3,'Higher educational establishment “International humanitarian and pedagogical institute “Beit-Hana”'),(4,'Kharkiv National I.P. Kotlyarevsky University of Arts'),(5,'Academy of Labour, Social Relations and Tourism'),(6,'Admiral Makarov National University of Shipbuilding'),(7,'Berdiansk University of management and business'),(8,'Bilotserkivskyi National Agrarian University'),(9,'Bogdan Khmelnitsky Melitopol State Pedagogical University (Melitopol, Zaporizhzhya region)'),(10,'Bogomolets National Medical University'),(11,'Borys Grinchenko Kyiv University'),(12,'Branch \"Kremenchuk of Privatee higher educational institution\" Dnepropetrovsk Alfred Nobel University \"'),(13,'Bukovinian State Medical University'),(14,'Central Ukrainian National Technical University, Kirovohrad'),(15,'Cherkasy State Business-College'),(16,'Cherkasy State Technological University'),(17,'Chernihiv Polytechnic National University'),(18,'Classical Private University'),(19,'Communal Health Institution \"Kharkiv Regional Medical College\"'),(20,'Danylo Halytsky Lviv National Medical University'),(21,'Diplomatic Academy of Ukraine, the Ministry of Foreign Affairs of Ukraine'),(22,'Dnipro National University of Railway Transport named after Academician V. Lazaryan'),(23,'Dnipro State Agrarian and Economic University'),(24,'Dnipro State Medical University'),(25,'Dnipro State Technical University'),(26,'Dnipropetrovsk State university of Internal Affairs'),(27,'Dniprovsky institute of the Private Joint Stock Company «Higher Educational Institution «Interregional Academy of Personnel Management»'),(28,'Donetsk National Medical University'),(29,'Donetsk National University of Economics and Trade named after Mykhailo Tugan-Baranovsky'),(30,'Donetsk State University of Management (DSUM)'),(31,'Dragomanov Ukrainian State University'),(32,'Drohobych Ivan Franko State Pedagogical University'),(33,'Flight Academy of the National Aviation University'),(34,'Higher Education Institution \"Kyiv Academy of Hairdressing Art\"'),(35,'Higher Education Institution Open International University of Human Development \"Ukraine\"'),(36,'Higher Educational Communal Institution of Lviv Regional Council “Andrei Krupynskyi Lviv Medical Academy'),(37,'Higher Educational Establishment of Ukoopspilka “Poltava University of Economics and Trade”'),(38,'Higher Educational Institution \"Alfred Nobel University\", Dnipro'),(39,'Higher Educational Institution \"King Danylo University\"'),(40,'Higher Educational Institution \"The Kiev Medical College\"'),(41,'Higher Educational Institution \"Ukrainian Catholic University\"'),(42,'Higher Educational Institution “Academician Yuriy Bugay International Scientific and Technical University”'),(43,'Higher education institution Kyiv Institute of Business and Technology LLC'),(44,'Hryhorii Skovoroda University in Pereiaslav'),(45,'Institute of Environmental Economics and Law'),(46,'Institute of Fire Safety named after the Heroes of Chornobyl'),(47,'International Humanitarian University'),(48,'International University of Finances'),(49,'Ivan Franko Lviv National University'),(50,'Ivan Horbachevsky Ternopil National Medical University of the Ministry of Health of Ukraine'),(51,'Ivan Kozhedub Kharkiv National Air Force University'),(52,'Ivano-Frankivsk National Medical University'),(53,'Ivano-Frankivsk National Technical University of Oil and Gas'),(54,'Izmail State University for the Humanities'),(55,'KROK University'),(56,'Kamianets-Podilskyi Ivan Ohiienko National University'),(57,'Kharkiv Economics and Law University'),(58,'Kharkiv Educational and Scientific Institute of SHEI “Banking University”'),(59,'Kharkiv Institute of Finance'),(60,'Kharkiv Institute of Trade and Economics of Kyiv National University of Trade and Economics'),(61,'Kharkiv Medical Academy of Postgraduate Education'),(62,'Kharkiv National Agrarian University named after V. Dokuchaev'),(63,'Kharkiv National Automobile and Highway University'),(64,'Kharkiv National Medical University'),(65,'Kharkiv National Pedagogical University named after H. Skovoroda'),(66,'Kharkiv National University of Civil Engineering and Architecture'),(67,'Kharkiv National University of Internal Affairs'),(68,'Kharkiv National University of Radio Electronics'),(69,'Kharkiv Petro Vasylenko National Technical University of Agriculture'),(70,'Kharkiv Regional Institute of Public Administration of the National Academy of Public Administration under the President of Ukraine'),(71,'Kharkiv State Academy of Culture'),(72,'Kharkiv State Academy of Physical Culture'),(73,'Kharkiv State Auto-Road College'),(74,'Kharkiv State University of Food Technology and Trade'),(75,'Kharkiv State Zooveterinary Academy'),(76,'Kharkiv University of Humanities \"People\'s Ukrainian Academy\"'),(77,'Kharkiv state Academy of design and arts'),(78,'Kherson National Technical University'),(79,'Kherson State Maritime Academy'),(80,'Kherson State University'),(81,'Kherson State agrarian and economic University'),(82,'Khmelnytskiy National University'),(83,'Khmelnytskyi Humanitarian-Pedagogical Academy'),(84,'Kremenchuk Flight College of Kharkiv National University of Internal Affairs'),(85,'Kremenchuk Mykhailo Ostrohradskyi National University'),(86,'Kryvyi Rih College of National Aviation University'),(87,'Kryvyi Rih State Pedagogical University (KSPU)'),(88,'Kyiv College of Construction, Architecture and Design'),(89,'Kyiv National Economic University named after Vadym Hetman'),(90,'Kyiv National I. K. Karpenko-Kary Theatre, Cinema and Television University'),(91,'Kyiv National Linguistic University'),(92,'Kyiv National University of Construction and Architecture'),(93,'Kyiv National University of Technologies and Design'),(94,'Kyiv National University of Trade and Economics'),(95,'Kyiv Slavonic University'),(96,'Kyiv municipal academy of circus and variety arts'),(97,'Leonid Yuzkov Khmelnytskyi University of Management and Law'),(98,'Lesya Ukrainka Volyn National University'),(99,'Limited liability Company \"Dnipro Medical Institute of Traditional and Nontraditional Medicine\"'),(100,'Luhansk National Agrarian University'),(101,'Luhansk State Medical University'),(102,'Lutsk national technical University'),(103,'Lviv National Academy of Arts'),(104,'Lviv National Environmental University'),(105,'Lviv National University of Veterinary Medicine and Biotechnology named after S.Z. Gzhytsky'),(106,'Lviv Polytechnic National University'),(107,'Lviv State University of Life Safety'),(108,'Lviv State University of Physical Culture'),(109,'Lviv University of Trade and Economics'),(110,'Mariupol State University'),(111,'Mukachevo State University'),(112,'Mykhailo Boichuk Kyiv State Academy of Decorative Applied Arts and Design'),(113,'Mykolayiv National Agrarian University'),(114,'Mykolayiv National University named after V. Sukhomlynskyi'),(115,'National Academy of Internal Affairs'),(116,'National Aerospace University \"Kharkiv Aviation Institute\"'),(117,'National Aviation University'),(118,'National Metallurgical Academy of Ukraine'),(119,'National Pirogov Memorial Medical University, Vinnytsia'),(120,'National Scientific Center «Hon. Prof. M. S. Bokarius Forensic Science Institute» of the Ministry of Justice of Ukraine'),(121,'National Technical University \"Kharkiv Polytechnic Institute\"'),(122,'National Technical University Dnipro Polytechnic'),(123,'National Technical University of Ukraine “Igor Sikorsky Kyiv Polytechnic Institute”'),(124,'National Transport University'),(125,'National University \"Odessa Law Academy\"'),(126,'National University \"Odessa Maritime Academy\"'),(127,'National University of \"Kyiv-Mohyla Academy\"'),(128,'National University of Civil Defence of Ukraine'),(129,'National University of Food Technologies'),(130,'National University of Life and Environmental Sciences of Ukraine'),(131,'National University of Pharmacy'),(132,'National University of Physical Education and Sport of Ukraine'),(133,'National University of Water and Environmental Engineering'),(134,'National University «Yuri Kondratyuk Poltava Polytechnic»'),(135,'National University “Zaporizhzhia Polytechnic”'),(136,'Nizhyn Mykola Gogol State University'),(137,'O.M. Beketov National University of Urban Economy'),(138,'Odesa National Academy of Telecommunications named after O. S. Popov'),(139,'Odesa National University of Technology'),(140,'Odesa State Academy of Technical Regulation and Quality'),(141,'Odesa institute of trade and economics of Kyiv National University of Trade and Economics'),(142,'Odessa I. I. Mechnikov National University'),(143,'Odessa Maritime College of fish industry named after O. Solyanyk'),(144,'Odessa National Economic University'),(145,'Odessa National Maritime University'),(146,'Odessa National Medical University'),(147,'Odessa National Music Academy named after A. V. Nezhdanova'),(148,'Odessa National Polytechnic University'),(149,'Odessa Regional Institute for Public Administration of the National Academy for Public Administration under the President of Ukraine'),(150,'Odessa State Academy of Civil Engineering and Architecture'),(151,'Odessa State Agrarian University'),(152,'Odessa State Environmental University'),(153,'Oleksandr Dovzhenko Hlukhiv National Pedagogical University'),(154,'Oles Honchar Dnipro National University'),(155,'Pavlo Tychyna Uman State Pedagogical University'),(156,'Petro Mohyla Black Sea National University'),(157,'Polissia National University'),(158,'Poltava Oil and Gas College of Poltava National Technical Yuri Kondratyuk University'),(159,'Poltava State Agrarian Academy'),(160,'Poltava State Medical University'),(161,'Poltava V.G. Korolenko National Pedagogical University'),(162,'PoltavaStateAgrarianUniversity'),(163,'Private Higher Education Institution\"International European University\"'),(164,'Private Higher Educational Establishment «Kharkiv International Medical University»'),(165,'Private Higher Educational Establishment “Kyiv Medical University”'),(166,'Private Higher Educational Establishment-Institute \"Ukrainian-American Concordia University\"'),(167,'Private Higher Educational Institution \"European University\"'),(168,'Private Higher Educational Institution \"International University of Economics and Humanities named after Stepan Demianchuk\"'),(169,'Private Higher Educational Institution \"Kyiv International University\"'),(170,'Private Higher Educational Institution \"Lviv University of Business and Law\"'),(171,'Private Higher Educational Institution \"Ukrainian Institute of Humanities\"'),(172,'Private Higher Educational Institution \"Vinnitsia Finance and Economics University\"'),(173,'Private Higher Educational Institution \"Zaporizhzhia Institute of Economics and Information Technologies\"'),(174,'Private Higher Educational Institution Financial-Legal College'),(175,'Private Institution of Higher Education \"Salvador Dali ACADEMY of Contemporary Arts\"'),(176,'Private Institution of Higher Education «Institute of Screen Arts»'),(177,'Private Institution of Higher Education “Rauf Ablyazov East European University”'),(178,'Private Joint Stock Company Higher Educational Institution \"Interregional Academy of Personnel Management\"'),(179,'Private establishment of higher education \"Dnipro Institute of medicine and public health\"'),(180,'Private higher education institution “International academy of ecology and medicine\"'),(181,'Private higher educational institution \"International University of Business and Law\"'),(182,'Prydniprovs\'ka State Academy of Civil Engineering and Architecture'),(183,'Prydniprovsk state academy of physical culture and sport'),(184,'Pylyp Orlyk International Classic University'),(185,'R.Glier Kyiv Institute of Music'),(186,'Rivne State University of Humanities'),(187,'Separate Structural Subdivision \"Ternopil Professional College of Ternopil Ivan Puluj National Technical University\"'),(188,'Shupyk National Healthcare University of Ukraine'),(189,'Simon Kuznets Kharkiv National University of Economics'),(190,'State Educational Institution \"Kyiv College of Communication\"'),(191,'State Higher Educational Institution \"Donbas State Pedagogical University\"'),(192,'State Higher Educational Institution \"Kharkiv College of textiles and design\"'),(193,'State Higher Educational Institution \"Kryvyi Rih National University\"'),(194,'State Higher Educational Institution \"National Forestry University of Ukraine\"'),(195,'State Higher Educational Institution \"Pryazovskyi State Technical University\"'),(196,'State Higher Educational Institution \"University Education Management\"'),(197,'State Higher Educational Institution “Banking University” Institute of Banking Technologies and Business (Kyiv)'),(198,'State Higher Educational Institution “Banking University” Lviv Institute'),(199,'State Higher Educational institution \"Ukrainian State University of Chemical Technology\"'),(200,'State Higher Educational institution \"Uzhhorod National University\"'),(201,'State Institution «Luhansk Taras Shevchenko National University»'),(202,'State University of Economics and Technology'),(203,'State University of Telecommunications'),(204,'Subsidiary Enterprise \"Kyiv Choreographic College\"'),(205,'Sumy Makarenko State Pedagogical University'),(206,'Sumy National Agrarian University'),(207,'Sumy State University'),(208,'Taras Shevchenko National University of Kyiv'),(209,'Taurida National V.I.Vernadsky University'),(210,'Tavria State Agrotechnological University'),(211,'Ternopil Ivan Puluj National Technical University'),(212,'Ternopil Volodymyr Hnatiuk National Pedagogical University'),(213,'The Bohdan Khmelnytskyy National University of Cherkasy'),(214,'The Filatov Institute of Eye Diseases and Tissue Therapy of The National Academy of Medical Sciences of Ukraine'),(215,'The Kharkiv Institute of the Private Joint Stock Company \"Higher education institution \"The Interregional Academy of Personnel Management \"'),(216,'The Mykola Lysenko Lviv National Music Academy'),(217,'The National Academy of Fine Art and Architecture'),(218,'The National Academy of Statistics, Accounting and Auditing'),(219,'The National University of Ostroh Academy'),(220,'The South Ukrainian National Pedagogical University named after K. D. Ushynsky'),(221,'The State Institution \"Institute of Neurosurgery named after acad. A.P.Romodanov of NAMS of Ukraine\"'),(222,'The Ukrainian National Tchaikovsky Academy of Music'),(223,'Ukrainian Academy of Printing'),(224,'Ukrainian Engineering Pedagogics Academy'),(225,'Ukrainian State Employment Service Training Institute'),(226,'Ukrainian State University of Railway Transport'),(227,'Ukrainian State University of Science and Technologies'),(228,'Uman National University of Horticulture'),(229,'University of Customs and Finance'),(230,'University of the State Fiscal Service of Ukraine'),(231,'V.N. Karazin Kharkiv National University'),(232,'Vasyl Stefanyk Precarpathian National University'),(233,'Vasyl\' Stus Donetsk National University'),(234,'Vinnytsia Mykhailo Kotsiubynskyi State Pedagogical University'),(235,'Vinnytsia National Agrarian University'),(236,'Vinnytsia National Technical University'),(237,'Volodymyr Dahl East Ukrainian National University'),(238,'Volodymyr Vynnychenko Central Ukrainian State Pedagogical University'),(239,'West Ukrainian National University'),(240,'Yaroslav Mudryi National Law University'),(241,'Yuriy Fedkovych Chernivtsi National University'),(242,'Zaporizhzhia National University'),(243,'Zaporizhzhia State Medical University'),(244,'Zaporizhzhya State Engineering Academy'),(245,'Zhytomyr Ivan Franko State University'),(246,'Zhytomyr Medical Institute of Zhytomyr Regional Council'),(247,'Zhytomyr Military Institute of S. P. Korolev of the State University of Telecommunications'),(248,'Zhytomyr State Technological University');
/*!40000 ALTER TABLE `universities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `email` varchar(255) NOT NULL,
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `photo` varchar(255) DEFAULT NULL,
  `status_id` int NOT NULL,
  `field_of_study_id` int DEFAULT NULL,
  `specialization_id` int DEFAULT NULL,
  `university_id` int DEFAULT NULL,
  `term` int DEFAULT NULL,
  `degree_id` int DEFAULT NULL,
  `role_id` int NOT NULL,
  `created_at` datetime NOT NULL,
  `is_banned` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  UNIQUE KEY `email` (`email`),
  UNIQUE KEY `username` (`username`),
  KEY `field_of_study_id` (`field_of_study_id`),
  KEY `specialization_id` (`specialization_id`),
  KEY `university_id` (`university_id`),
  KEY `status_id` (`status_id`),
  KEY `role_id` (`role_id`),
  KEY `degree_id` (`degree_id`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`field_of_study_id`) REFERENCES `fields_of_study` (`id`),
  CONSTRAINT `users_ibfk_2` FOREIGN KEY (`specialization_id`) REFERENCES `specializations` (`id`),
  CONSTRAINT `users_ibfk_3` FOREIGN KEY (`university_id`) REFERENCES `universities` (`id`),
  CONSTRAINT `users_ibfk_4` FOREIGN KEY (`status_id`) REFERENCES `statuses` (`id`),
  CONSTRAINT `users_ibfk_5` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`),
  CONSTRAINT `users_ibfk_6` FOREIGN KEY (`degree_id`) REFERENCES `degrees` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'oleksalviv@lviv.ua','OleksaLviv','password','http://gossip.byethost7.com/Icons/oleksa.jpg',1,12,44,106,2,1,1,'2024-11-19 12:17:52',0),(2,'GARDAR@GARD.COM','GARDAR_GARD','PASSWORD','http://gossip.byethost7.com/Icons/gardar.jpg',1,12,44,106,2,1,2,'2024-11-19 12:22:31',0),(3,'moneyless@gmail.com','olliinyk','gagagaga','http://gossip.byethost7.com/Icons/ivan.jpg',1,12,44,106,2,1,1,'2024-11-19 12:27:57',1),(5,'socutesosweet@pookie.com','kartoplyana','coquette','http://gossip.byethost7.com/Icons/polina.jpg',2,12,44,106,NULL,6,2,'2024-11-19 12:32:33',0),(6,'o@zelinskyi.gov.ua','o_zelinskyi','potyzhnist','http://gossip.byethost7.com/Icons/sashko.jpg',3,12,NULL,NULL,NULL,NULL,2,'2024-11-19 12:36:03',0),(7,'yurii.stelmakh.pz.2023@lpnu.ua','stelmakh_yurii','password','http://gossip.byethost7.com/Icons/yarA.jpg',1,12,44,106,2,1,2,'2024-11-19 12:38:19',0),(8,'mariia_ltvn@gmail.com','mariia_kolos','password','http://gossip.byethost7.com/Icons/marichka.jpg',1,12,44,106,2,1,1,'2024-11-19 12:42:14',0),(9,'andrii.potikha.pz.2023@lpnu.ua','apitlp','password','http://gossip.byethost7.com/Icons/andriy.jpg',1,12,44,106,1,1,1,'2024-11-19 12:43:58',0),(10,'kurapov@phd.ua','pavlo_kurapov','phdphdphd','phd',2,11,43,211,NULL,5,1,'2024-11-19 12:46:17',1),(11,'cat.frisky@acode.tk','frisky','tktktktk','frisky',1,15,122,106,2,2,1,'2024-11-19 12:51:18',0),(15,'email@email.com','test_testy','12345678',NULL,3,28,125,106,NULL,6,1,'2024-11-25 12:38:49',0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users_to_votes`
--

DROP TABLE IF EXISTS `users_to_votes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users_to_votes` (
  `user_id` int NOT NULL,
  `topic_id` int DEFAULT NULL,
  `reply_id` int DEFAULT NULL,
  `vote` int NOT NULL,
  KEY `users_to_votes_replies_id_fk` (`reply_id`),
  KEY `users_to_votes_topics_id_fk` (`topic_id`),
  KEY `users_to_votes_users_id_fk` (`user_id`),
  CONSTRAINT `users_to_votes_replies_id_fk` FOREIGN KEY (`reply_id`) REFERENCES `replies` (`id`),
  CONSTRAINT `users_to_votes_topics_id_fk` FOREIGN KEY (`topic_id`) REFERENCES `topics` (`id`),
  CONSTRAINT `users_to_votes_users_id_fk` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`),
  CONSTRAINT `check_vote` CHECK ((`vote` in (-(1),0,1)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users_to_votes`
--

LOCK TABLES `users_to_votes` WRITE;
/*!40000 ALTER TABLE `users_to_votes` DISABLE KEYS */;
INSERT INTO `users_to_votes` VALUES (7,NULL,6,1),(9,10,NULL,-1);
/*!40000 ALTER TABLE `users_to_votes` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-12-09 17:14:29

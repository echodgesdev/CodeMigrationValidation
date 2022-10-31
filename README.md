Overview
===============

The Code Migration Validator is designed to validate Code Migration Tickets (tickets used by DevOps for the release of SoX applications). 
This project is inspired by the workflow I've seen in my professional experience and can be adapted by modifying the ExpectedCMTCrawler.cs and ActualCMTCrawler.cs classes.

Code Migration Process
-----------------------
1. Product dictates what features should exist in the sprint and send out an email with the feature numbers.
2. Developers merge the code to the appropriate branch in source control. For scripts, Developers ensure they are in the proper file location. These are not managed in source control.
3. Developers add their features to the Code Migration Ticket in a system called "Foundation" . 
	* Adding a feature means adding the feature number and adding the latest build of the code. For SQL scripts, this means adding the directory where the SQL script exists.
	* Only one build should exist in foundation for each project but Foundation doesn't prevent you from having multiple. It's under the developer's discretion to find duplicates.
4. Once all features have been added to the CMT, the CMT requires DEV and Product sign-off. Dev must note that the contents of the CM is correct. Product must agree that the feature #s are correct.
5. Once Dev and Product sign off, QA accepts the CM and it is migrated to the appropriate testing environment.

This tool simplifies steps 2-4 by validating that the expected WIs, the most recent code builds, and all existing scripts are found within the CM. It also validates that all scripts are written in the correct format as defined in the AppSettings.json file.


Configure (Basic)
---------------------
1. Copy Tool To Local Machine (this will not run on the fileshare server).
2. Open AppSettings.json and configure the following:
	* CMTNumber -- this is the Foundation Ticket Number
	* FeatureNumbers -- this is an array of Feature Numbers from AzureDevOps

To Run:
-----------------
1. Double click on CodeMigrationValidator.exe
2. Results will print out on the screen and will be written to a results_{cmtNumber}_[currentDateTime].txt file in the code validator directory.


Dev Notes
==============

Build
---------
1. Manually update version.
2. Update release notes above.
3. Open Visual studo and build Debug Solution.
2. Copy contents of Bin Directory to the deployment location in a version specific folder (follow the naming convention seen)-- \\isgfile02\DevTools\ProsUtilities\ChrisUtilities\CodeMigrationValidator

Future Work Items/Room for improvepment
=============================================
* Change console App to simple REST API.
* Building the "Expected CMT" via code and doing a comparison could simply be streamlined to auto building the CMT and replace steps 3 and 4. Options/Ideas:
	(a) Create REST API called once all code is merged that creates a CMT request and POSTs it to Foundation.
		(b) Triggered via a slack command as part of process
		(c) Triggered on build of QA branches and updates the existing CM; This doesn't work for resources not in source control such as SQL scripts

Release Notes v1.0
====================
* Validates that all Feature Numbers Listed in AppSettings.json are in the Foundation CM
* Validates Server Contents In CM by getting latest build from build server and assuming this is the expected version in Foundation CM.
	- The date range for these builds are based on the created date of the Foundation CM and it's expected release date
* Validates Client Contents In CM by getting latest build from build server and assuming this is the expected version in Foundation CM
	- The date range for these builds are based on the created date of the Foundation CM and it's expected release date
* Given a list of all Features expected in CM, validates that the features with SQL/Mongo scripts have the scripts added in the CM
* Validates that Scripts follow patterns specified in AppSettings.json





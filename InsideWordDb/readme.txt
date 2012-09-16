Introduction:
--------------
This readme will explain the various sql scripts found in this folder.


createSchema.sql:
------------------
The complete database schema (without data) for the InsideWord database.
To generate a script to populate a blank db with the schema with the below options,
go to MS SQL Server Management Studio, right click on the db in object explorer,
choose task -> generate scripts, follow the wizard and set the below options.
Note these options are under advanced in the 'set scripting options' step.
This schema is built with the following options (* indicates modified from default):

ANSI Padding:                          True
Append to File:                        False
Continue scripting on Error:           False
Convert UDDTs to Base Types:           False
Generate Script for Dependent Objects: True
Include Descriptive Headers:           True
Include If NOT EXISTS:                 False
Include system constraint names:       False
Include unsupported statements:        False
Schema qualify object names:           True
Script Bindings:                       False
Script Collation:                      False
Script Defaults:                       True
Script DROP and CREATE:                Script DROP and CREATE*
Script Extended Properties:            True
Script for Server Version:             SQL Server 2008 R2
Script for the database engine type:   Stand-alone instance
Script Logins:                         True*
Script Object-Level Permissions:       False
Script Statistics:                     Do not script statistics
Script USE DATABASE:                   True
Types of data to script:               Schema only

Script Change Tracking:                False
Script Check Constraints:              True
Script Data Compression Options:       False
Script Foreign Keys:                   True
Script Full-Text Indexes:              True*
Script Indexes:                        True
Script Primary Keys:                   True
Script Triggers:                       True*
Script Unique Keys:                    True


clearDatabase.sql:
-------------------
Script to empty the database of all data and to reset the key identity values.
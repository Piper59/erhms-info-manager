CREATE TABLE [Global] (
	[Identity] INTEGER NOT NULL IDENTITY PRIMARY KEY,
	Name TEXT NOT NULL,
	[Value] TEXT NOT NULL
);

/*
SELECT *
FROM Gender;
*/

CREATE TABLE Gender (
	GenderId NVARCHAR(255) NOT NULL PRIMARY KEY,
	Name TEXT NOT NULL,
	Pronouns TEXT NOT NULL
);

/*
SELECT *
FROM Person;
*/

CREATE TABLE Person (
	PersonId NVARCHAR(255) NOT NULL PRIMARY KEY,
	GenderId NVARCHAR(255) NOT NULL REFERENCES Gender (GenderId),
	/*
	FirstName TEXT NOT NULL,
	LastName TEXT NOT NULL,
	*/
	Name TEXT NOT NULL,
	BirthDate DATETIME,
	Height FLOAT,
	Weight FLOAT
);

/*
foo;
bar;
baz;
*/

INSERT INTO [Global] (Name, [Value]) VALUES ('Version', '1.0');

INSERT INTO Gender (GenderId, Name, Pronouns) VALUES ('273c6d62-be89-48df-9e04-775125bc4f6a', 'Male', 'he;him;his;his');
INSERT INTO Gender (GenderId, Name, Pronouns) VALUES ('a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Female', 'she;her;her;hers');
/* INSERT INTO Gender (GenderId, Name, Pronouns) VALUES ('db312453-49db-4345-9ed6-e33f1d6e899e', 'Neuter', 'it;it;its;its'); */

INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('999181b4-8445-e585-5178-74a9e11e75fa', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Graham', '1986-09-14', 5.8, 180.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('c15c66cf-b6c9-08a4-1c24-552105cac021', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Collier', '1985-05-15', 5.4, 195.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('7959ee0f-588f-089f-c78d-5fc3ca07b1fc', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Sampson', '1983-12-20', 5.2, 170.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('83fd4286-2589-1e99-9d13-9dfe94b7faa3', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Fry', '1988-01-02', 5.2, 204.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('dd94e7f1-9b4c-e7c0-63b9-23230a4e6225', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Phelps', '1980-10-28', 6.1, 182.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('d0379c44-72f3-f319-3b4e-449eecf94022', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Hoover', '1987-01-19', 4.3, 114.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('1175949e-1eb2-46bc-3dbe-a568311661a6', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Burns', '1987-02-27', 5.5, 161.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('b903fc4f-b1bb-a56e-07f7-b620d050d662', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Lowe', '1985-12-28', 5.8, 166.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('763131b0-38dc-095e-4bbc-6d235065c0c2', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Dennis', '1980-03-05', 4.6, 156);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('d2ce3d9d-2c95-034d-700a-12979458a7cc', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Saunders', '1981-03-04', 5.5, 209.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('844a5db5-7657-a0cb-fbc5-cc79424cb1ba', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Pace', '1985-07-12', 6.1, 190.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('3b8aedfa-9f0e-99f9-faee-5aa46755c503', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Hebert', '1986-06-16', 4.7, 176.7);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('06d0e31f-bbf4-6470-778f-e70f8faf7605', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Craft', '1987-03-05', 4.1, 128.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('11be1a49-7021-a5e0-d87f-ce2ed2f99a52', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Valencia', '1985-07-07', 5.2, 193.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('48ccf89f-bf9e-d5ba-ae6c-83d159849377', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Cameron', '1985-05-22', 6, 208.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('a89c24cf-b9c8-7fc8-fe64-80daf89f8896', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Buchanan', '1986-04-17', 4.9, 177);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('099d39ef-8fb7-6fb7-ee0c-badab7579042', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Hart', '1984-11-02', 5.1, 153.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('ba71d098-1b84-dcc6-4bee-884e67bc2147', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Jefferson', '1984-03-31', 5.8, 114.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('1b0dfc8e-8189-4af0-c443-0fc22c553e02', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Berry', '1984-01-30', 5.6, 213.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('ea81532c-8eed-6c7c-bcf1-8741bead7118', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Zimmerman', '1989-12-20', 5.3, 214.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('e59a5363-300f-1867-dcac-5f1d51708e35', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Puckett', '1982-09-10', 5.9, 175.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('74b694e0-0d5e-d15b-b344-8f15bf87b56e', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Baldwin', '1989-03-28', 5.5, 124.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('bd30e714-446b-ae27-f404-ddeeb77a3705', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Conley', '1980-11-23', 5.2, 168.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('4504d1a0-9c8b-10be-a481-adb80d22043b', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Swanson', '1985-09-08', 5.4, 139.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('50aacf9c-7cbb-3d70-773b-71af382ed01f', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Mcmillan', '1987-02-12', 4.7, 183.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('8447abc8-061a-6ba8-0b06-f87b88568c95', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Hutchinson', '1983-03-06', 5.2, 158.7);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('7440b322-a783-aa08-0bc5-dc39f53bda5f', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Doyle', '1986-12-18', 5.3, 175);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('5af9fd68-27c9-ead5-9add-7006314936ec', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Francis', '1987-11-10', 4.8, 186.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('95ee8f41-21e1-f947-c0c3-adb02887d80d', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Greene', '1980-12-23', 6.3, 225.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('2bffbfe3-7ec8-9cfc-0ca1-3d9008463c85', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Camacho', '1982-05-21', 4.8, 161.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('f15f860f-c67c-cb0a-3924-4c196a282382', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Barber', '1987-12-31', 5.6, 182.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('7ca4aaee-23c9-949e-7ac7-502fe6a4f207', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Small', '1987-09-02', 5.8, 170.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('efaf3aef-f676-2e82-aaca-7dfed31b15e9', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Harmon', '1982-10-31', 5.7, 152.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('2ef0fdad-af12-2157-a0b2-323d6594d894', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Cole', '1980-10-14', 4.5, 150.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('8dd10d02-f654-539a-48ad-24b6486ba5f9', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Frederick', '1987-12-14', 5.6, 199.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('49370068-e786-7d98-7e6d-8de378758efc', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Mcdaniel', '1985-04-10', 5.5, 187.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('4b2b9748-f528-48aa-6c45-4a0522d36c54', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Chan', '1980-06-21', 5.3, 192.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('986a56c8-8694-b02d-97a8-3a4cb4b94663', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Jackson', '1986-09-04', 5.3, 156.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('fa31f1e8-788d-93b5-45d2-3213ddbe713d', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Sloan', '1983-03-04', 5.4, 196);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('7ec4b8c6-23e8-b58d-d92d-a319a34031b0', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Beach', '1989-09-07', 5.9, 193);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('db58e517-b205-0805-0e48-637b75ea462a', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Snow', '1982-03-03', 5.4, 168.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('1e8f8c49-2987-9ece-6727-3903bd29de7d', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Osborn', '1989-07-08', 5.3, 174.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('4c698c83-3ca0-beee-0ed7-5942102a908f', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Nixon', '1986-08-14', 5.5, 195.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('1f698e52-cc41-288b-304b-2dceafaac921', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Johnson', '1984-06-17', 5.9, 206.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('d082c6ac-caba-f72c-a8f3-a4177e5e61a9', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Garner', '1981-06-16', 6, 175.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('498cfd5f-e1a1-0a0e-95ec-e55f5599ba72', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Colon', '1988-10-31', 5.9, 150.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('0e786258-193a-bfe6-1b41-465b92421b6b', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Russell', '1980-04-06', 5.6, 165.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('39dc6250-beb3-a921-4c37-26d1a73b2e10', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Guerra', '1982-11-18', 5.7, 161.7);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('a9fb5756-1682-a4a7-8440-1e776ee9d782', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Dodson', '1988-12-08', 5.6, 168.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('e85b3095-9272-aaf3-0820-c0c157eb8e38', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Dixon', '1984-04-26', 5.7, 128);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('db99dbe6-2ac9-8063-d0e7-cbeda40b02c2', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Deleon', '1987-12-25', 5.1, 138.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('4be7fc78-19c0-0330-d38c-4effdd837fa6', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Harrell', '1989-10-15', 4.6, 164.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('ee192d7d-395d-7fee-11b8-21b919fbf02e', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Reilly', '1984-09-29', 5.7, 185.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('5b8289f4-084c-8e5c-51b6-112c7fcae2e0', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Riddle', '1987-02-25', 5.3, 180.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('ddb859e8-03ab-54b3-388c-ee21db383ea7', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Stephenson', '1984-06-09', 5.9, 190.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('9f3ebbaf-9705-600f-b3d9-83540f9d15ec', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Tucker', '1987-04-27', 5.8, 152.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('d47810e3-f534-40ed-1b96-42ae37bf22a2', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Blair', '1980-09-20', 5.5, 175.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('16ca4da5-e59d-caa3-2f48-265de37855e8', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Wheeler', '1983-04-08', 4.9, 171.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('115e8fb1-e3ac-6361-e812-8fd110ca3d88', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Humphrey', '1981-10-25', 5.6, 141.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('cb8e1e87-89be-3bb4-4ed9-c199daded83b', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Small', '1982-01-09', 5.7, 167.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('22eade18-460f-f0c7-46f8-f5a3109a5f8f', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Cleveland', '1983-01-07', 5.5, 188.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('54418e52-7268-7540-7d5b-992ada31079f', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Finch', '1985-07-31', 5.1, 164.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('35ae0bbb-53fb-9e23-7bf3-c984486fd461', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Patton', '1988-06-23', 6.4, 174.1);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('de939c19-25dc-38c7-c1bf-06aba5bd9a63', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Richardson', '1987-01-18', 5.1, 172.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('2c82e185-e800-ea5c-3d9c-523086357494', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Yates', '1985-02-07', 4.9, 176.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('852d3417-3989-fd34-5ebb-ea53bb3741df', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Austin', '1989-05-15', 5.1, 158);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('fd98d05d-60de-8044-a4ff-022de291ce29', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Dillard', '1987-10-22', 5.6, 159.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('4f518974-9660-b4f6-d603-79cda1144960', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Hancock', '1982-05-16', 5.3, 196);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('62e6b86b-3585-bf5c-6f17-b7b8d781f6e7', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'David', '1983-12-12', 5.6, 168);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('74f9aff8-1fb9-0dd3-71f3-f32f391d96ae', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Cleveland', '1988-05-22', 5.7, 192.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('96ea1f38-06e9-ac2a-7247-ba332a9fcf19', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Norman', '1985-06-01', 5.6, 208.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('4cba1982-135f-fc8b-a863-bc5b33c3b89f', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Wood', '1982-07-22', 4.9, 176.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('bf8fdb37-e025-1793-eb1e-6df8e28b20c4', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Hughes', '1985-06-19', 5.8, 144.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('ab892ebf-0006-7e4f-8bce-fbc658c45077', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Conway', '1985-09-10', 5.4, 162);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('80c8657d-bd3e-9134-4326-92f16cad4e43', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Goff', '1980-06-30', 6.2, 198.7);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('965d9682-d065-af0c-6881-bf16c52fbbe2', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Hewitt', '1988-11-15', 5.2, 191.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('cf34ff74-d41e-9198-1039-a229f9c97f32', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Dunlap', '1983-02-19', 5.9, 190.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('bb4c0c73-7fe7-1e68-499c-f9a4a632f7f5', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Mason', '1980-09-18', 5.2, 173.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('363b196d-95ff-a9f7-6d93-94d19496bbd5', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Morgan', '1988-07-25', 5.5, 183.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('9edfc111-e075-f229-0e1d-2a810960569c', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Mcconnell', '1989-02-20', 6.4, 167);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('09e9b432-d94f-0bc0-db74-41f4fc626e4e', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Cunningham', '1985-08-18', 5.3, 194.8);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('d017569b-8f32-e992-56e0-0ffe4763905e', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Mcmillan', '1980-08-03', 6, 183);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('5c4dd5db-7aa2-c3a4-308d-a0df5243866a', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Clarke', '1988-09-09', 6, 160.6);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('4a42b219-5ae3-ab15-a69f-1ba0b0e6db95', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Price', '1981-08-22', 5.5, 142.3);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('12221a3d-23e1-eeef-06f3-f44aab52d7ec', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Frye', '1989-08-21', 5.2, 198.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('21b46716-bc96-a604-8370-84082b75faad', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Thornton', '1981-11-16', 5.8, 183.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('9fc87a72-9603-6f2a-c6de-84f5b9d6d32e', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Berg', '1981-05-20', 5.9, 150.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('c88fabb3-00e4-ebc5-a5f6-f5351ca6e1ec', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Roberts', '1983-02-16', 5.9, 171.1);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('8508cb61-b016-6202-e142-9e3600b27910', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Knapp', '1987-03-06', 5.4, 176);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('22083fba-5f19-9484-778a-2be528166d89', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Spence', '1982-11-26', 5.5, 223.2);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('2e617569-8872-4ff1-bc8c-08afc9817f7a', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Vega', '1985-02-05', 6, 221.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('b2cfc57a-584a-58d4-7347-b0f605469735', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Burnett', '1989-06-16', 5.9, 164.7);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('7cf4eab6-7c33-6528-7c17-c7bc7f22c62a', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Bell', '1989-12-17', 6.1, 183.1);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('91787903-9fcf-5c78-c34b-8152c96727ea', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Sims', '1980-03-02', 5.7, 167.9);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('879917ff-a6e2-2cbc-30cb-8d95203b6cf9', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Fisher', '1983-08-13', 5.2, 206.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('afa218eb-ebaf-ae09-8499-4c55e09f2d70', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Sawyer', '1989-06-18', 5.1, 164.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('abad6854-4680-2b5b-43a9-5e9880192c5c', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Mccoy', '1983-09-23', 6, 164.5);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('d715ec54-e063-a27b-ee78-09716087c4b8', 'a7d96f3a-a990-4619-82d0-fcd9a9629f31', 'Joseph', '1983-03-15', 5, 165.4);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('bb37c258-4601-ef5e-8037-fe4c8ed249b0', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Buckley', '1980-07-15', 4.6, 195);
INSERT INTO Person (PersonId, GenderId, Name, BirthDate, Height, Weight) VALUES ('86f7d328-eb3b-ebb6-c934-fffead2d71c6', '273c6d62-be89-48df-9e04-775125bc4f6a', 'Ramos', '1980-09-09', 6.5, 140.3);

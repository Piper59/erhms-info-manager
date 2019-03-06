CREATE TABLE ERHMS_UniquePairs (
	UniquePairId NVARCHAR(255) NOT NULL PRIMARY KEY,
	Responder1Id NVARCHAR(255) NOT NULL REFERENCES Responders (GlobalRecordId),
	Responder2Id NVARCHAR(255) NOT NULL REFERENCES Responders (GlobalRecordId) 
);

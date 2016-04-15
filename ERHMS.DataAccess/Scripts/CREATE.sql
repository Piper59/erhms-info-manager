ALTER TABLE Responders
ALTER COLUMN GlobalRecordId VARCHAR(255) NOT NULL UNIQUE;

CREATE TABLE ERHMS_Incidents (
	IncidentId VARCHAR(255) NOT NULL PRIMARY KEY,
	Name TEXT NOT NULL,
	Description TEXT,
	Phase VARCHAR(255) NOT NULL,
	StartDate DATETIME,
	EndDateEstimate DATETIME,
	EndDateActual DATETIME
);

CREATE TABLE ERHMS_Locations (
	LocationId VARCHAR(255) NOT NULL PRIMARY KEY,
	IncidentId VARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId),
	Name VARCHAR(255) NOT NULL,
	Description TEXT NOT NULL,
	Address TEXT NOT NULL,
	Latitude FLOAT,
	Longitude FLOAT
);

CREATE TABLE ERHMS_Registrations (
	RegistrationId VARCHAR(255) NOT NULL PRIMARY KEY,
	ResponderId VARCHAR(255) NOT NULL REFERENCES Responders (GlobalRecordId),
	IncidentId VARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

CREATE TABLE ERHMS_Assignments (
	AssignmentId VARCHAR(255) NOT NULL PRIMARY KEY,
	ViewId INTEGER NOT NULL REFERENCES metaViews (ViewId),
	ResponderId VARCHAR(255) NOT NULL REFERENCES Responders (GlobalRecordId)
);

CREATE TABLE ERHMS_ViewLinks (
	ViewLinkId VARCHAR(255) NOT NULL PRIMARY KEY,
	ViewId INTEGER NOT NULL REFERENCES metaViews (ViewId),
	IncidentId VARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

CREATE TABLE ERHMS_PgmLinks (
	PgmLinkId VARCHAR(255) NOT NULL PRIMARY KEY,
	PgmId INTEGER REFERENCES metaPrograms (ProgramId),
	Path TEXT,
	IncidentId VARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

CREATE TABLE ERHMS_CanvasLinks (
	CanvasLinkId VARCHAR(255) NOT NULL PRIMARY KEY,
	Path TEXT NOT NULL,
	IncidentId VARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

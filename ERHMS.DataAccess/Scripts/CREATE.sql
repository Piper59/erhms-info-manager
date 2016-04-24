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
	Name TEXT NOT NULL,
	Description TEXT,
	Address TEXT,
	Latitude FLOAT,
	Longitude FLOAT
);

CREATE TABLE ERHMS_Rosters (
	RosterId VARCHAR(255) NOT NULL PRIMARY KEY,
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
	PgmId INTEGER NOT NULL REFERENCES metaPrograms (ProgramId),
	IncidentId VARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

CREATE TABLE ERHMS_CanvasLinks (
	CanvasLinkId VARCHAR(255) NOT NULL PRIMARY KEY,
	CanvasId INTEGER NOT NULL REFERENCES metaCanvases (CanvasId),
	IncidentId VARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

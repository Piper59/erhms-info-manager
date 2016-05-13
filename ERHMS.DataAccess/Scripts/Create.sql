ALTER TABLE Responders
ADD CONSTRAINT UQ_Responders_GlobalRecordId UNIQUE (GlobalRecordId);

CREATE TABLE ERHMS_Incidents (
	IncidentId NVARCHAR(255) NOT NULL PRIMARY KEY,
	Name TEXT NOT NULL,
	Description TEXT,
	Phase NVARCHAR(255) NOT NULL,
	StartDate DATETIME,
	EndDateEstimate DATETIME,
	EndDateActual DATETIME,
	Deleted BIT NOT NULL
);

CREATE TABLE ERHMS_IncidentNotes (
	IncidentNoteId NVARCHAR(255) NOT NULL PRIMARY KEY,
	IncidentId NVARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId),
	Content TEXT NOT NULL,
	[Date] DATETIME NOT NULL
);

CREATE TABLE ERHMS_Rosters (
	RosterId NVARCHAR(255) NOT NULL PRIMARY KEY,
	ResponderId NVARCHAR(255) NOT NULL REFERENCES Responders (GlobalRecordId),
	IncidentId NVARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

CREATE TABLE ERHMS_Locations (
	LocationId NVARCHAR(255) NOT NULL PRIMARY KEY,
	IncidentId NVARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId),
	Name TEXT NOT NULL,
	Description TEXT,
	Address TEXT,
	Latitude FLOAT,
	Longitude FLOAT
);

CREATE TABLE ERHMS_Assignments (
	AssignmentId NVARCHAR(255) NOT NULL PRIMARY KEY,
	ViewId INTEGER NOT NULL REFERENCES metaViews (ViewId),
	ResponderId NVARCHAR(255) NOT NULL REFERENCES Responders (GlobalRecordId)
);

CREATE TABLE ERHMS_ViewLinks (
	ViewLinkId NVARCHAR(255) NOT NULL PRIMARY KEY,
	ViewId INTEGER NOT NULL REFERENCES metaViews (ViewId),
	IncidentId NVARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

CREATE TABLE ERHMS_PgmLinks (
	PgmLinkId NVARCHAR(255) NOT NULL PRIMARY KEY,
	PgmId INTEGER NOT NULL REFERENCES metaPrograms (ProgramId),
	IncidentId NVARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

CREATE TABLE ERHMS_CanvasLinks (
	CanvasLinkId NVARCHAR(255) NOT NULL PRIMARY KEY,
	CanvasId INTEGER NOT NULL REFERENCES metaCanvases (CanvasId),
	IncidentId NVARCHAR(255) NOT NULL REFERENCES ERHMS_Incidents (IncidentId)
);

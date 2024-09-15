CREATE DATABASE Docentify;
USE Docentify;

CREATE TABLE IF NOT EXISTS Institutions
(
    institutionId INT          NOT NULL AUTO_INCREMENT,
    name          VARCHAR(150) NOT NULL,
    PRIMARY KEY (institutionId)
);

CREATE TABLE IF NOT EXISTS Users
(
    userId        INT          NOT NULL AUTO_INCREMENT,
    name          VARCHAR(150) NOT NULL,
    birthDate     DATETIME     NOT NULL,
    email         VARCHAR(100) NOT NULL,
    telephone     VARCHAR(45)  NOT NULL,
    document      VARCHAR(45)  NOT NULL,
    isInstitution TINYINT      NOT NULL DEFAULT 0,
    institutionId INT          NOT NULL,
    PRIMARY KEY (userId),
    FOREIGN KEY (institutionId)
        REFERENCES Institutions (institutionId)
);

CREATE TABLE IF NOT EXISTS PasswordHashes
(
    hashedPassword INT NOT NULL,
    salt           INT NULL,
    userId         INT NOT NULL,
    PRIMARY KEY (hashedPassword),
    FOREIGN KEY (userId)
        REFERENCES Users (userId)
);

CREATE TABLE IF NOT EXISTS Courses
(
    course_id     INT         NOT NULL AUTO_INCREMENT,
    name          VARCHAR(45) NOT NULL,
    institutionId INT         NOT NULL,
    PRIMARY KEY (course_id),
    FOREIGN KEY (institutionId)
        REFERENCES Institutions (institutionId)
);

CREATE TABLE IF NOT EXISTS FavoriteCourses
(
    courseId     INT      NOT NULL,
    userId       INT      NOT NULL,
    favoriteDate DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (courseId, userId),
    FOREIGN KEY (courseId)
        REFERENCES Courses (course_id),
    FOREIGN KEY (userId)
        REFERENCES Users (userId)
);

CREATE TABLE IF NOT EXISTS Enrollments
(
    enrollmentId   INT      NOT NULL AUTO_INCREMENT,
    userId         INT      NOT NULL,
    courseId       INT      NOT NULL,
    enrollmentDate DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    isRequired     TINYINT  NULL DEFAULT 0,
    requiredDate   DATETIME NULL,
    PRIMARY KEY (enrollmentId),
    FOREIGN KEY (userId)
        REFERENCES Users (userId),
    FOREIGN KEY (courseId)
        REFERENCES Courses (course_id)
);

CREATE TABLE IF NOT EXISTS Steps
(
    stepId   INT         NOT NULL AUTO_INCREMENT,
    stepName VARCHAR(45) NOT NULL,
    courseId INT         NOT NULL,
    PRIMARY KEY (stepId),
    FOREIGN KEY (courseId)
        REFERENCES Courses (course_id)
);

CREATE TABLE IF NOT EXISTS VideoSteps
(
    videoStepId INT          NOT NULL AUTO_INCREMENT,
    videoUrl    VARCHAR(250) NOT NULL,
    stepId      INT          NOT NULL,
    PRIMARY KEY (videoStepId, stepId),
    FOREIGN KEY (stepId)
        REFERENCES Steps (stepId)
);

CREATE TABLE IF NOT EXISTS Activities
(
    activityId INT NOT NULL AUTO_INCREMENT,
    stepId     INT NOT NULL,
    PRIMARY KEY (activityId, stepId),
    FOREIGN KEY (stepId)
        REFERENCES Steps (stepId)
);

CREATE TABLE IF NOT EXISTS UserProgress
(
    userId       INT      NOT NULL,
    courseIid    INT      NOT NULL,
    stepId       INT      NOT NULL,
    progressDate DATETIME NOT NULL,
    PRIMARY KEY (userId, courseIid),
    FOREIGN KEY (userId)
        REFERENCES Users (userId),
    FOREIGN KEY (courseIid)
        REFERENCES Courses (course_id),
    FOREIGN KEY (stepId)
        REFERENCES Steps (stepId)
);

CREATE TABLE IF NOT EXISTS Questions
(
    questionId INT  NOT NULL AUTO_INCREMENT,
    statement  TEXT NOT NULL,
    activityId INT  NOT NULL,
    PRIMARY KEY (questionId, activityId),
    FOREIGN KEY (activityId)
        REFERENCES Activities (activityId)
);

CREATE TABLE IF NOT EXISTS Options
(
    answerIid  INT     NOT NULL AUTO_INCREMENT,
    text       TEXT    NOT NULL,
    isCorrect  TINYINT NULL DEFAULT 0,
    questionId INT     NOT NULL,
    PRIMARY KEY (answerIid, questionId),
    FOREIGN KEY (questionId)
        REFERENCES Questions (questionId)
);

CREATE TABLE IF NOT EXISTS FileSteps
(
    fileStepId INT  NOT NULL AUTO_INCREMENT,
    fileData   BLOB NOT NULL,
    stepId     INT  NOT NULL,
    PRIMARY KEY (fileStepId, stepId),
    FOREIGN KEY (stepId)
        REFERENCES Steps (stepId)
);

CREATE TABLE IF NOT EXISTS UserScores
(
    userId INT NOT NULL,
    score  INT NULL DEFAULT 0,
    PRIMARY KEY (userId),
    FOREIGN KEY (userId)
        REFERENCES Users (userId)
);

CREATE TABLE IF NOT EXISTS CourseStyles
(
    styleId  INT         NOT NULL,
    courseId INT         NOT NULL,
    pageId   VARCHAR(45) NOT NULL,
    style    LONGTEXT    NULL,
    PRIMARY KEY (styleId, pageId),
    FOREIGN KEY (courseId)
        REFERENCES Courses (course_id)
);

CREATE TABLE IF NOT EXISTS UserPreferences
(
    preferenceName VARCHAR(50) NOT NULL,
    defaultValue   VARCHAR(45) NOT NULL,
    PRIMARY KEY (preferenceName)
);

CREATE TABLE IF NOT EXISTS UserPreferencesValues
(
    userId         INT         NOT NULL,
    preferenceName VARCHAR(50) NOT NULL,
    value          VARCHAR(45) NOT NULL,
    PRIMARY KEY (userId, preferenceName),
    FOREIGN KEY (userId)
        REFERENCES Users (userId),
    FOREIGN KEY (preferenceName)
        REFERENCES UserPreferences (preferenceName)
);
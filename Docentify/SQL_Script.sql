DROP DATABASE IF EXISTS Docentify;
CREATE DATABASE IF NOT EXISTS Docentify DEFAULT CHARACTER SET utf8;
USE Docentify;

DROP TABLE IF EXISTS Users;
CREATE TABLE IF NOT EXISTS Users
(
    id        INT          NOT NULL AUTO_INCREMENT,
    name      VARCHAR(150) NOT NULL,
    birthDate DATE         NOT NULL,
    email     VARCHAR(100) NOT NULL,
    telephone VARCHAR(45)  NULL,
    document  VARCHAR(45)  NOT NULL,
    PRIMARY KEY (id),
    UNIQUE INDEX email_UNIQUE (email ASC) VISIBLE,
    UNIQUE INDEX document_UNIQUE (document ASC) VISIBLE
);

DROP TABLE IF EXISTS UserPasswordHashes;
CREATE TABLE IF NOT EXISTS UserPasswordHashes
(
    id             INT          NOT NULL AUTO_INCREMENT,
    hashedPassword VARCHAR(200) NOT NULL,
    salt           VARCHAR(200) NOT NULL,
    userId         INT          NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS UserScores;
CREATE TABLE IF NOT EXISTS UserScores
(
    userId INT NOT NULL,
    score  INT NULL DEFAULT 0,
    PRIMARY KEY (userId),
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS UserPreferences;
CREATE TABLE IF NOT EXISTS UserPreferences
(
    id             INT         NOT NULL AUTO_INCREMENT,
    preferenceName VARCHAR(50) NOT NULL,
    defaultValue   VARCHAR(45) NOT NULL,
    PRIMARY KEY (id)
);

DROP TABLE IF EXISTS UserPreferencesValues;
CREATE TABLE IF NOT EXISTS UserPreferencesValues
(
    userId       INT         NOT NULL,
    preferenceId INT         NOT NULL,
    value        VARCHAR(45) NOT NULL,
    PRIMARY KEY (userId, preferenceId),
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE,
    FOREIGN KEY (preferenceId) REFERENCES UserPreferences (id)
);

DROP TABLE IF EXISTS Institutions;
CREATE TABLE IF NOT EXISTS Institutions
(
    id        INT          NOT NULL AUTO_INCREMENT,
    name      VARCHAR(150) NOT NULL,
    email     VARCHAR(100) NOT NULL,
    telephone VARCHAR(45)  NULL,
    PRIMARY KEY (id),
    UNIQUE INDEX email_UNIQUE (email ASC) VISIBLE,
    UNIQUE INDEX name_UNIQUE (name ASC) VISIBLE
);

DROP TABLE IF EXISTS InstitutionPasswordHashes;
CREATE TABLE IF NOT EXISTS InstitutionPasswordHashes
(
    id             VARCHAR(45)  NOT NULL,
    hashedPassword VARCHAR(200) NOT NULL,
    salt           VARCHAR(200) NOT NULL,
    institutionId  INT          NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (institutionId) REFERENCES Institutions (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Associations;
CREATE TABLE IF NOT EXISTS Associations
(
    userId        INT NOT NULL,
    institutionId INT NOT NULL,
    PRIMARY KEY (userId, institutionId),
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE,
    FOREIGN KEY (institutionId) REFERENCES Institutions (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Courses;
CREATE TABLE IF NOT EXISTS Courses
(
    id            INT         NOT NULL AUTO_INCREMENT,
    name          VARCHAR(45) NOT NULL,
    description   TEXT        NULL,
    institutionId INT         NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (institutionId) REFERENCES Institutions (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS FavoritedCourses;
CREATE TABLE IF NOT EXISTS FavoritedCourses
(
    courseId     INT      NOT NULL,
    userId       INT      NOT NULL,
    favoriteDate DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (courseId, userId),
    FOREIGN KEY (courseId) REFERENCES Courses (id)
        ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Enrollments;
CREATE TABLE IF NOT EXISTS Enrollments
(
    id             INT      NOT NULL AUTO_INCREMENT,
    enrollmentDate DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    isRequired     TINYINT  NULL DEFAULT 0,
    requiredDate   DATETIME NULL,
    userId         INT      NOT NULL,
    courseId       INT      NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE,
    FOREIGN KEY (courseId) REFERENCES Courses (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Steps;
CREATE TABLE IF NOT EXISTS Steps
(
    id          INT         NOT NULL AUTO_INCREMENT,
    `order`     INT         NOT NULL,
    description VARCHAR(45) NOT NULL,
    type        INT         NOT NULL,
    courseId    INT         NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (courseId) REFERENCES Courses (id)
);


DROP TABLE IF EXISTS UserProgress;
CREATE TABLE IF NOT EXISTS UserProgress
(
    enrollment_id INT      NOT NULL,
    stepId        INT      NOT NULL,
    progressDate  DATETIME NOT NULL,
    PRIMARY KEY (enrollment_id, stepId),
    FOREIGN KEY (stepId) REFERENCES Steps (id),
    FOREIGN KEY (enrollment_id) REFERENCES Enrollments (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS VideoSteps;
CREATE TABLE IF NOT EXISTS VideoSteps
(
    id     INT          NOT NULL AUTO_INCREMENT,
    url    VARCHAR(250) NOT NULL,
    stepId INT          NOT NULL,
    PRIMARY KEY (id, stepId),
    FOREIGN KEY (stepId) REFERENCES Steps (id)
);

DROP TABLE IF EXISTS Activities;
CREATE TABLE IF NOT EXISTS Activities
(
    id     INT NOT NULL AUTO_INCREMENT,
    stepId INT NOT NULL,
    PRIMARY KEY (id, stepId),
    FOREIGN KEY (stepId) REFERENCES Steps (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Questions;
CREATE TABLE IF NOT EXISTS Questions
(
    id         INT  NOT NULL AUTO_INCREMENT,
    statement  TEXT NOT NULL,
    activityId INT  NOT NULL,
    PRIMARY KEY (id, activityId),
    FOREIGN KEY (activityId) REFERENCES Activities (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Options;
CREATE TABLE IF NOT EXISTS Options
(
    id         INT     NOT NULL AUTO_INCREMENT,
    text       TEXT    NOT NULL,
    isCorrect  TINYINT NULL DEFAULT 0,
    questionId INT     NOT NULL,
    PRIMARY KEY (id, questionId),
    FOREIGN KEY (questionId) REFERENCES Questions (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS FileSteps;
CREATE TABLE IF NOT EXISTS FileSteps
(
    id     INT  NOT NULL AUTO_INCREMENT,
    data   BLOB NOT NULL,
    stepId INT  NOT NULL,
    PRIMARY KEY (id, stepId),
    FOREIGN KEY (stepId) REFERENCES Steps (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS CourseStyles;
CREATE TABLE IF NOT EXISTS CourseStyles
(
    id       INT         NOT NULL AUTO_INCREMENT,
    name     VARCHAR(45) NULL,
    courseId INT         NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (courseId) REFERENCES Courses (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS StyleVariables;
CREATE TABLE IF NOT EXISTS StyleVariables
(
    id           INT         NOT NULL AUTO_INCREMENT,
    name         VARCHAR(45) NOT NULL,
    defaultValue VARCHAR(45) NULL,
    PRIMARY KEY (id),
    UNIQUE INDEX name_UNIQUE (name ASC) VISIBLE
);

DROP TABLE IF EXISTS StyleVariablesValues;
CREATE TABLE IF NOT EXISTS StyleVariablesValues
(
    style_id    INT         NOT NULL,
    variable_id INT         NOT NULL,
    value       VARCHAR(45) NOT NULL,
    PRIMARY KEY (style_id, variable_id),
    FOREIGN KEY (style_id) REFERENCES CourseStyles (id),
    FOREIGN KEY (variable_id) REFERENCES StyleVariables (id)
);

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
    INDEX fk_User_Organization_idx (institutionId ASC) VISIBLE,
    CONSTRAINT fk_User_Organization
        FOREIGN KEY (institutionId)
            REFERENCES Institutions (institutionId)
);

CREATE TABLE IF NOT EXISTS PasswordHashes
(
    hashedPassword INT NOT NULL,
    salt           INT NULL,
    userId         INT NOT NULL,
    PRIMARY KEY (hashedPassword),
    INDEX fk_PasswordHashes_User1_idx (userId ASC) VISIBLE,
    CONSTRAINT fk_PasswordHashes_User1
        FOREIGN KEY (userId)
            REFERENCES Users (userId)
);

CREATE TABLE IF NOT EXISTS Courses
(
    course_id     INT         NOT NULL AUTO_INCREMENT,
    name          VARCHAR(45) NOT NULL,
    institutionId INT         NOT NULL,
    PRIMARY KEY (course_id),
    INDEX fk_Courses_Organization1_idx (institutionId ASC) VISIBLE,
    CONSTRAINT fk_Courses_Organization1
        FOREIGN KEY (institutionId)
            REFERENCES Institutions (institutionId)
);

CREATE TABLE IF NOT EXISTS FavoritedCourses
(
    courseId     INT      NOT NULL,
    userId       INT      NOT NULL,
    favoriteDate DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (courseId, userId),
    INDEX fk_Courses_has_User_User1_idx (userId ASC) VISIBLE,
    INDEX fk_Courses_has_User_Courses1_idx (courseId ASC) VISIBLE,
    CONSTRAINT fk_Courses_has_User_Courses1
        FOREIGN KEY (courseId)
            REFERENCES Courses (course_id),
    CONSTRAINT fk_Courses_has_User_User1
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
    INDEX fk_User_has_Courses_User1_idx (userId ASC) VISIBLE,
    INDEX fk_Enrollment_Courses1_idx (courseId ASC) VISIBLE,
    PRIMARY KEY (enrollmentId),
    CONSTRAINT fk_User_has_Courses_User1
        FOREIGN KEY (userId)
            REFERENCES Users (userId),
    CONSTRAINT fk_Enrollment_Courses1
        FOREIGN KEY (courseId)
            REFERENCES Courses (course_id)
);

CREATE TABLE IF NOT EXISTS Steps
(
    stepId   INT         NOT NULL AUTO_INCREMENT,
    stepName VARCHAR(45) NOT NULL,
    courseId INT         NOT NULL,
    PRIMARY KEY (stepId),
    INDEX fk_Steps_Courses1_idx (courseId ASC) VISIBLE,
    CONSTRAINT fk_Steps_Courses1
        FOREIGN KEY (courseId)
            REFERENCES Courses (course_id)
);

CREATE TABLE IF NOT EXISTS VideoSteps
(
    videoStepId INT          NOT NULL AUTO_INCREMENT,
    videoUrl    VARCHAR(250) NOT NULL,
    stepId      INT          NOT NULL,
    PRIMARY KEY (videoStepId, stepId),
    INDEX fk_VideoStep_Steps1_idx (stepId ASC) VISIBLE,
    CONSTRAINT fk_VideoStep_Steps1
        FOREIGN KEY (stepId)
            REFERENCES Steps (stepId)
);

CREATE TABLE IF NOT EXISTS Activities
(
    activityId INT NOT NULL AUTO_INCREMENT,
    stepId     INT NOT NULL,
    PRIMARY KEY (activityId, stepId),
    INDEX fk_Activities_Steps1_idx (stepId ASC) VISIBLE,
    CONSTRAINT fk_Activities_Steps1
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
    INDEX fk_UserProgress_Users1_idx (userId ASC) VISIBLE,
    INDEX fk_UserProgress_Courses1_idx (courseIid ASC) VISIBLE,
    INDEX fk_UserProgress_Steps1_idx (stepId ASC) VISIBLE,
    CONSTRAINT fk_UserProgress_Users1
        FOREIGN KEY (userId)
            REFERENCES Users (userId),
    CONSTRAINT fk_UserProgress_Courses1
        FOREIGN KEY (courseIid)
            REFERENCES Courses (course_id),
    CONSTRAINT fk_UserProgress_Steps1
        FOREIGN KEY (stepId)
            REFERENCES Steps (stepId)
);

CREATE TABLE IF NOT EXISTS Questions
(
    questionId INT  NOT NULL AUTO_INCREMENT,
    statement  TEXT NOT NULL,
    activityId INT  NOT NULL,
    PRIMARY KEY (questionId, activityId),
    INDEX fk_Questions_Activities1_idx (activityId ASC) VISIBLE,
    CONSTRAINT fk_Questions_Activities1
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
    INDEX fk_Options_Questions1_idx (questionId ASC) VISIBLE,
    CONSTRAINT fk_Options_Questions1
        FOREIGN KEY (questionId)
            REFERENCES Questions (questionId)
);

CREATE TABLE IF NOT EXISTS FileSteps
(
    fileStepId INT  NOT NULL AUTO_INCREMENT,
    fileData   BLOB NOT NULL,
    stepId     INT  NOT NULL,
    PRIMARY KEY (fileStepId, stepId),
    INDEX fk_VideoStep_Steps1_idx (stepId ASC) VISIBLE,
    CONSTRAINT fk_VideoStep_Steps10
        FOREIGN KEY (stepId)
            REFERENCES Steps (stepId)
);

CREATE TABLE IF NOT EXISTS UserScores
(
    userId INT NOT NULL,
    score  INT NULL DEFAULT 0,
    PRIMARY KEY (userId),
    INDEX fk_Users_copy1_Users1_idx (userId ASC) VISIBLE,
    CONSTRAINT fk_Users_copy1_Users1
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
    INDEX fk_CourseStyles_Courses1_idx (courseId ASC) VISIBLE,
    CONSTRAINT fk_CourseStyles_Courses1
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
    INDEX fk_Users_has_UserPreferences_UserPreferences1_idx (preferenceName ASC) VISIBLE,
    INDEX fk_Users_has_UserPreferences_Users1_idx (userId ASC) VISIBLE,
    CONSTRAINT fk_Users_has_UserPreferences_Users1
        FOREIGN KEY (userId)
            REFERENCES Users (userId),
    CONSTRAINT fk_Users_has_UserPreferences_UserPreferences1
        FOREIGN KEY (preferenceName)
            REFERENCES UserPreferences (preferenceName)
);
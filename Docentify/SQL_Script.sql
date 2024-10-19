DROP DATABASE IF EXISTS Docentify;
CREATE DATABASE IF NOT EXISTS Docentify DEFAULT CHARACTER SET utf8;
USE Docentify;

DROP TABLE IF EXISTS Users;
CREATE TABLE IF NOT EXISTS Users
(
    id            INT          NOT NULL AUTO_INCREMENT,
    name          VARCHAR(150) NOT NULL,
    birthDate     DATE         NOT NULL,
    email         VARCHAR(100) NOT NULL,
    telephone     VARCHAR(45)  NULL,
    gender        CHAR(2)      NULL,
    document      VARCHAR(45)  NOT NULL,
    creation_date DATETIME     NULL DEFAULT CURRENT_TIMESTAMP,
    update_date   DATETIME     NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE INDEX email_UNIQUE (email ASC) VISIBLE,
    UNIQUE INDEX document_UNIQUE (document ASC) VISIBLE
);

DROP TABLE IF EXISTS Institutions;
CREATE TABLE IF NOT EXISTS Institutions
(
    id        INT          NOT NULL AUTO_INCREMENT,
    name      VARCHAR(150) NOT NULL,
    email     VARCHAR(100) NOT NULL,
    telephone VARCHAR(45)  NULL,
    address   VARCHAR(350) NULL,
    document  VARCHAR(45)  NOT NULL,
    creation_date DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    update_date   DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE INDEX email_UNIQUE (email ASC) VISIBLE,
    UNIQUE INDEX name_UNIQUE (name ASC) VISIBLE,
    UNIQUE INDEX document_UNIQUE (document ASC) VISIBLE
);

DROP TABLE IF EXISTS UserPasswordHashes;
CREATE TABLE IF NOT EXISTS UserPasswordHashes
(
    id             INT NOT NULL AUTO_INCREMENT,
    hashedPassword VARCHAR(100)         NOT NULL,
    salt           VARCHAR(100)         NOT NULL,
    userId         INT         NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Courses;
CREATE TABLE IF NOT EXISTS Courses
(
    id            INT         NOT NULL AUTO_INCREMENT,
    name          VARCHAR(45) NOT NULL,
    description   TEXT        NULL,
    institutionId INT         NOT NULL,
    isRequired    BIT  NULL DEFAULT 0,
    requiredTimeLimit INT    NULL DEFAULT 30,
    creationDate DATETIME    NULL DEFAULT CURRENT_TIMESTAMP,
    updateDate   DATETIME    NULL DEFAULT CURRENT_TIMESTAMP,
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
    isActive       BIT      NULL DEFAULT 1,
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
    id          INT          NOT NULL AUTO_INCREMENT,
    `order`       INT        NOT NULL,
    title       VARCHAR(45)  NOT NULL,
    description VARCHAR(500) NOT NULL,
    type        INT          NOT NULL,
    content     TEXT         NOT NULL,
    courseId    INT          NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (courseId) REFERENCES Courses (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Activities;
CREATE TABLE IF NOT EXISTS Activities
(
    id     INT NOT NULL AUTO_INCREMENT,
    allowedTries INT NOT NULL DEFAULT 3,
    stepId INT NOT NULL,
    PRIMARY KEY (id, stepId),
    FOREIGN KEY (stepId) REFERENCES Steps (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS UserProgress;
CREATE TABLE IF NOT EXISTS UserProgress
(
    enrollment_id INT      NOT NULL,
    stepId        INT      NOT NULL,
    progressDate  DATETIME NOT NULL,
    PRIMARY KEY (enrollment_id, stepId),
    FOREIGN KEY (stepId) REFERENCES Steps (id)
        ON DELETE CASCADE,
    FOREIGN KEY (enrollment_id) REFERENCES Enrollments (id)
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

DROP TABLE IF EXISTS UserScores;
CREATE TABLE IF NOT EXISTS UserScores
(
    userId INT NOT NULL,
    score  INT NULL DEFAULT 0,
    PRIMARY KEY (userId),
    FOREIGN KEY (userId) REFERENCES Users (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS CourseStyles;
CREATE TABLE IF NOT EXISTS CourseStyles
(
    id       INT         NOT NULL AUTO_INCREMENT,
    name     VARCHAR(45) NULL,
    courseId INT         NOT NULL,
    isRequired TINYINT  NULL DEFAULT 0,
    PRIMARY KEY (id),
    FOREIGN KEY (courseId) REFERENCES Courses (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS UserPreferences;
CREATE TABLE IF NOT EXISTS UserPreferences
(
    id             INT         NOT NULL AUTO_INCREMENT,
    name VARCHAR(50) NOT NULL,
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
    FOREIGN KEY (style_id) REFERENCES CourseStyles (id)
        ON DELETE CASCADE,
    FOREIGN KEY (variable_id) REFERENCES StyleVariables (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS InstitutionPasswordHashes;
CREATE TABLE IF NOT EXISTS InstitutionPasswordHashes
(
    id             INT NOT NULL AUTO_INCREMENT,
    hashedPassword VARCHAR(100)         NOT NULL,
    salt           VARCHAR(100)         NOT NULL,
    institutionId  INT         NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (institutionId) REFERENCES Institutions (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS Cards;
CREATE TABLE IF NOT EXISTS Cards
(
    id                   INT          NOT NULL AUTO_INCREMENT,
    nome                 VARCHAR(45)  NOT NULL,
    descricao            VARCHAR(250) NULL,
    silhouette_image_url VARCHAR(200) NOT NULL,
    achieved_image_url   VARCHAR(200) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE INDEX nome_UNIQUE (nome ASC) VISIBLE
);

DROP TABLE IF EXISTS UserCards;
CREATE TABLE IF NOT EXISTS UserCards
(
    user_id          INT      NOT NULL,
    card_id          INT      NOT NULL,
    acquirement_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, card_id),
    FOREIGN KEY (user_id) REFERENCES Users (id)
        ON DELETE CASCADE,
    FOREIGN KEY (card_id) REFERENCES Cards (id)
        ON DELETE CASCADE
);

DROP TABLE IF EXISTS UserNotifications;
CREATE TABLE IF NOT EXISTS UserNotifications
(
    id                INT          NOT NULL AUTO_INCREMENT,
    user_id           INT          NOT NULL,
    text              VARCHAR(250) NOT NULL,
    date              DATETIME     NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id, user_id),
    FOREIGN KEY (user_id) REFERENCES Users (id)
        ON DELETE CASCADE
);

-- Inserir usuários
INSERT INTO Users (name, birthDate, email, telephone, document)
VALUES
    ('João da Silva', '1985-07-20', 'joao.silva@example.com', '11912345678', '12345678901'),
    ('Maria Oliveira', '1990-11-15', 'maria.oliveira@example.com', '11987654321', '23456789012'),
    ('Pedro Santos', '1988-03-25', 'pedro.santos@example.com', '11965432198', '34567890123'),
    ('Ana Paula', '1975-05-10', 'ana.paula@example.com', '11987654329', '98765432100'),
    ('Carlos Pereira', '1982-09-12', 'carlos.pereira@example.com', '11923456789', '45678901234'),
    ('Fernanda Lima', '1995-01-25', 'fernanda.lima@example.com', '11954321876', '56789012345'),
    ('Roberto Nunes', '1992-04-18', 'roberto.nunes@example.com', '11987651234', '67890123456'),
    ('Juliana Alves', '1987-12-03', 'juliana.alves@example.com', '11965439876', '78901234567'),
    ('Eduardo Ramos', '1978-11-22', 'eduardo.ramos@example.com', '11934567890', '89012345678'),
    ('Camila Rodrigues', '1993-08-30', 'camila.rodrigues@example.com', '11923459876', '90123456789');

-- Inserir senhas de usuários
INSERT INTO UserPasswordHashes (hashedPassword, salt, userId)
VALUES
    ('hash1', 'salt1', 1),
    ('hash2', 'salt2', 2),
    ('hash3', 'salt3', 3),
    ('hash4', 'salt4', 4),
    ('hash5', 'salt5', 5),
    ('hash6', 'salt6', 6),
    ('hash7', 'salt7', 7),
    ('hash8', 'salt8', 8),
    ('hash9', 'salt9', 9),
    ('hash10', 'salt10', 10);

-- Inserir instituições
INSERT INTO Institutions (name, email, telephone, document, address)
VALUES
    ('Universidade Federal', 'contato@ufederal.edu.br', '1134567890', '231', 'Rua tal'),
    ('Instituto de Educação Superior', 'contato@iesup.com.br', '1187654321', '123', 'Rua tal'),
    ('Faculdade Privada', 'contato@facprivada.com.br', '1143216789', '321', 'Rua tal');

-- Inserir cursos
INSERT INTO Courses (name, description, isRequired, requiredTimeLimit, institutionId)
VALUES
    ('Curso de Pedagogia', 'Capacitação em metodologias educacionais modernas', 0, 0,  1),
    ('Tecnologias Educacionais', 'Curso focado no uso de tecnologia em sala de aula', 0, 0, 1),
    ('Capacitação em Metodologias Ativas', 'Uso de metodologias ativas no ensino superior', 1, 30, 2),
    ('Inovação na Educação', 'Ferramentas inovadoras para o ensino superior', 1, 60, 3);

-- Inserir matrículas de usuários em cursos
INSERT INTO Enrollments (enrollmentDate, userId, courseId)
VALUES
    ('2024-09-01', 1, 1),
    ('2024-09-01', 2, 2),
    ('2024-08-20', 3, 3),
    ('2024-09-10', 4, 4),
    ('2024-09-05', 5, 1),
    ('2024-09-02', 6, 2),
    ('2024-08-25', 7, 3),
    ('2024-09-08', 8, 4),
    ('2024-09-03', 9, 1),
    ('2024-09-07', 10, 2);

-- Inserir pontuações de usuários
INSERT INTO UserScores (userId, score)
VALUES
    (1, 85),
    (2, 90),
    (3, 70),
    (4, 95),
    (5, 88),
    (6, 92),
    (7, 75),
    (8, 89),
    (9, 87),
    (10, 91);

-- Inserir preferências de usuários
INSERT INTO UserPreferences (name, defaultValue)
VALUES
    ('Preferência de Tema', 'Claro'),
    ('Notificações', 'Ativado'),
    ('Lembrar login', 'Desativado');

-- Inserir etapas dos cursos
INSERT INTO Steps (`order`, title, description, type, courseId, content)
VALUES
    (1, 'Introdução ao curso', 'Introdução', 1, 1, 'texto'),
    (2, 'Módulo 1: Ferramentas Tecnológicas', 'Módulo 1: Ferramentas Tecnológicas', 2, 1, 'url video'),
    (1, 'Aula inicial', 'Aula inicial', 1, 2, 'texto'),
    (2, 'Módulo 1: Metodologias Ativas', 'Módulo 1: Metodologias Ativas', 3, 3, 'explicação antes da atividade'),
    (1, 'Aula inaugural', 'Aula inaugural', 1, 4, 'texto');

-- Inserir progresso dos usuários nos cursos
INSERT INTO UserProgress (enrollment_id, stepId, progressDate)
VALUES
    (1, 1, '2024-09-02'),
    (1, 2, '2024-09-05'),
    (2, 1, '2024-09-04'),
    (2, 2, '2024-09-06'),
    (3, 1, '2024-08-22'),
    (4, 1, '2024-09-11'),
    (5, 1, '2024-09-06'),
    (6, 1, '2024-09-03'),
    (7, 1, '2024-08-26'),
    (8, 1, '2024-09-09'),
    (9, 1, '2024-09-04'),
    (10, 1, '2024-09-08');
USE FinalSteamDB
GO

-- First, drop all tables in correct order (dependent tables first)
DROP TABLE IF EXISTS UserLikedComment
DROP TABLE IF EXISTS UserLikedPost
DROP TABLE IF EXISTS UserDislikedComment
DROP TABLE IF EXISTS UserDislikedPost
DROP TABLE IF EXISTS ForumComments
DROP TABLE IF EXISTS ForumPosts
DROP TABLE IF EXISTS NewsRatings
DROP TABLE IF EXISTS NewsComments
DROP TABLE IF EXISTS NewsPosts
DROP TABLE IF EXISTS CHAT_INVITES
DROP TABLE IF EXISTS FriendRequests
DROP TABLE IF EXISTS Friends
DROP TABLE IF EXISTS Reviews
DROP TABLE IF EXISTS Feature_User
DROP TABLE IF EXISTS OwnedGames_Collection
DROP TABLE IF EXISTS UserAchievements
DROP TABLE IF EXISTS Achievements
DROP TABLE IF EXISTS SoldGames
DROP TABLE IF EXISTS ReviewsGiven
DROP TABLE IF EXISTS ReviewsReceived
DROP TABLE IF EXISTS Posts
DROP TABLE IF EXISTS Features
DROP TABLE IF EXISTS Collections
DROP TABLE IF EXISTS OwnedGames
DROP TABLE IF EXISTS ReviewsUsers
DROP TABLE IF EXISTS Games
DROP TABLE IF EXISTS UserSessions
DROP TABLE IF EXISTS PasswordResetCodes
DROP TABLE IF EXISTS UserProfiles
DROP TABLE IF EXISTS Wallet
DROP TABLE IF EXISTS PointsOffers
DROP TABLE IF EXISTS Friendships
DROP TABLE IF EXISTS FriendUsers
DROP TABLE IF EXISTS ChatUsers
DROP TABLE IF EXISTS Users
DROP TABLE IF EXISTS ChatConversations
DROP TABLE IF EXISTS ChatMessages

---------------------------------------------------------------------------------------------------
-- Users - maintained original data types
---------------------------------------------------------------------------------------------------
CREATE TABLE Users (
    user_id         INT IDENTITY(1,1) PRIMARY KEY,
    username        NVARCHAR(50) COLLATE SQL_Latin1_General_CP1254_CS_AS NOT NULL UNIQUE,  -- case-sensitive usernames
    email           NVARCHAR(100) COLLATE SQL_Latin1_General_CP1254_CS_AS NOT NULL UNIQUE, -- case-sensitive emails
    hashed_password NVARCHAR(255) NOT NULL,
    developer       BIT NOT NULL DEFAULT 0,
    created_at      DATETIME NOT NULL DEFAULT GETDATE(),
    last_login      DATETIME NULL
);
GO

---------------------------------------------------------------------------------------------------
-- Alternative User Tables (renamed to ChatUsers to avoid case sensitivity issues)
---------------------------------------------------------------------------------------------------
CREATE TABLE ChatUsers(
    userid INT PRIMARY KEY,
    username NVARCHAR(50),
    ipAddress VARCHAR(50)
);
GO

CREATE TABLE FriendUsers (
    UserId INT PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    ProfilePhotoPath NVARCHAR(255) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

---------------------------------------------------------------------------------------------------
-- User Sessions
---------------------------------------------------------------------------------------------------
CREATE TABLE UserSessions (
    session_id  UNIQUEIDENTIFIER PRIMARY KEY,
    user_id     INT NOT NULL,
    created_at  DATETIME NOT NULL DEFAULT GETDATE(),
    expires_at  DATETIME NOT NULL,
    CONSTRAINT FK_UserSessions_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- User Profiles
---------------------------------------------------------------------------------------------------
CREATE TABLE UserProfiles (
    profile_id       INT IDENTITY(1,1) PRIMARY KEY,
    user_id          INT NOT NULL UNIQUE,
    profile_picture  NVARCHAR(255),
    bio              NVARCHAR(1000),
    equipped_frame   NVARCHAR(255),
    equipped_hat     NVARCHAR(255),
    equipped_pet     NVARCHAR(255),
    equipped_emoji   NVARCHAR(255),
    last_modified    DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Password Reset Codes
---------------------------------------------------------------------------------------------------
CREATE TABLE PasswordResetCodes (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    user_id         INT NOT NULL,
    reset_code      NVARCHAR(6) NOT NULL,
    expiration_time DATETIME NOT NULL,
    used            BIT NOT NULL DEFAULT 0,
    email           NVARCHAR(255),
    CONSTRAINT FK_PasswordResetCodes_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Wallet
---------------------------------------------------------------------------------------------------
CREATE TABLE Wallet (
    wallet_id       INT IDENTITY(1,1) PRIMARY KEY,
    user_id         INT NOT NULL UNIQUE,
    points          INT NOT NULL DEFAULT 0,
    money_for_games DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_Wallet_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Points Offers
---------------------------------------------------------------------------------------------------
CREATE TABLE PointsOffers (
    offer_id        INT IDENTITY(1,1) PRIMARY KEY,
    numberOfPoints  INT NOT NULL,
    value           INT NOT NULL
);
GO

---------------------------------------------------------------------------------------------------
-- Friendships
---------------------------------------------------------------------------------------------------
CREATE TABLE Friendships (
    friendship_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id       INT NOT NULL,
    friend_id     INT NOT NULL,
    CONSTRAINT FK_Friendships_User   FOREIGN KEY (user_id)   REFERENCES Users(user_id),
    CONSTRAINT FK_Friendships_Friend FOREIGN KEY (friend_id) REFERENCES Users(user_id),
    CONSTRAINT UQ_Friendship         UNIQUE (user_id, friend_id),
    CONSTRAINT CHK_FriendshipUsers   CHECK (user_id != friend_id)
);

CREATE INDEX IX_Friendships_UserId   ON Friendships(user_id);
CREATE INDEX IX_Friendships_FriendId ON Friendships(friend_id);
GO

---------------------------------------------------------------------------------------------------
-- Game-related tables
---------------------------------------------------------------------------------------------------
CREATE TABLE Games (
    GameId INT PRIMARY KEY, 
    Title NVARCHAR(100) NOT NULL, 
    Description NVARCHAR(MAX), 
    ReleaseDate DATE, 
    CoverImage VARBINARY(MAX)
);
GO

CREATE TABLE ReviewsUsers ( 
   UserId INT PRIMARY KEY, 
   Name NVARCHAR(100) NOT NULL, 
   ProfilePicture VARBINARY(MAX)
);
GO

---------------------------------------------------------------------------------------------------
-- Collections
---------------------------------------------------------------------------------------------------
CREATE TABLE Collections (
    collection_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id       INT NOT NULL,
    name          NVARCHAR(100) NOT NULL CHECK (LEN(name) BETWEEN 1 AND 100),
    cover_picture NVARCHAR(255) CHECK (
                       cover_picture LIKE '%.svg' OR
                       cover_picture LIKE '%.png' OR
                       cover_picture LIKE '%.jpg'
                     ),
    is_public     BIT NOT NULL DEFAULT 1,
    created_at    DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    CONSTRAINT FK_Collections_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Owned Games
---------------------------------------------------------------------------------------------------
CREATE TABLE OwnedGames (
    game_id       INT IDENTITY(1,1) PRIMARY KEY,
    user_id       INT NOT NULL,
    title         NVARCHAR(100) NOT NULL CHECK (LEN(title) BETWEEN 1 AND 100),
    description   NVARCHAR(MAX),
    cover_picture NVARCHAR(255) CHECK (
                       cover_picture LIKE '%.svg' OR
                       cover_picture LIKE '%.png' OR
                       cover_picture LIKE '%.jpg'
                     ),
    CONSTRAINT FK_OwnedGames_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Owned Games Collection Link with Triggers instead of Cascading Constraints
---------------------------------------------------------------------------------------------------
CREATE TABLE OwnedGames_Collection (
    collection_id INT NOT NULL,
    game_id       INT NOT NULL,
    PRIMARY KEY (collection_id, game_id),
    CONSTRAINT FK_OGC_Collections FOREIGN KEY (collection_id)
        REFERENCES Collections(collection_id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_OGC_OwnedGames FOREIGN KEY (game_id)
        REFERENCES OwnedGames(game_id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

-- Create triggers to implement cascading behavior
CREATE TRIGGER trg_Collections_Delete
ON Collections
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM OwnedGames_Collection
    WHERE collection_id IN (SELECT collection_id FROM DELETED);
END;
GO

CREATE TRIGGER trg_OwnedGames_Delete
ON OwnedGames
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM OwnedGames_Collection
    WHERE game_id IN (SELECT game_id FROM DELETED);
END;
GO

CREATE TRIGGER trg_Collections_Update
ON Collections
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(collection_id)
    BEGIN
        UPDATE ogc
        SET collection_id = i.collection_id
        FROM OwnedGames_Collection ogc
        JOIN DELETED d ON ogc.collection_id = d.collection_id
        JOIN INSERTED i ON d.user_id = i.user_id;
    END
END;
GO

CREATE TRIGGER trg_OwnedGames_Update
ON OwnedGames
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(game_id)
    BEGIN
        UPDATE ogc
        SET game_id = i.game_id
        FROM OwnedGames_Collection ogc
        JOIN DELETED d ON ogc.game_id = d.game_id
        JOIN INSERTED i ON d.user_id = i.user_id;
    END
END;
GO

---------------------------------------------------------------------------------------------------
-- Features
---------------------------------------------------------------------------------------------------
CREATE TABLE Features (
    feature_id  INT IDENTITY(1,1) PRIMARY KEY,
    name        NVARCHAR(100) NOT NULL,
    value       INT NOT NULL CHECK (value >= 0),
    description NVARCHAR(255),
    type        NVARCHAR(50) CHECK (type IN ('frame', 'emoji', 'background', 'pet', 'hat')),
    source      NVARCHAR(255) CHECK (
                   source LIKE '%.svg' OR
                   source LIKE '%.png' OR
                   source LIKE '%.jpg'
                 )
);
GO

---------------------------------------------------------------------------------------------------
-- Feature Ownership
---------------------------------------------------------------------------------------------------
CREATE TABLE Feature_User (
    user_id    INT NOT NULL,
    feature_id INT NOT NULL,
    equipped   BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (user_id, feature_id),
    CONSTRAINT FK_FeatureUser_Users FOREIGN KEY (user_id) 
        REFERENCES Users(user_id) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE,
    CONSTRAINT FK_FeatureUser_Features FOREIGN KEY (feature_id) 
        REFERENCES Features(feature_id) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Achievements
---------------------------------------------------------------------------------------------------
CREATE TABLE Achievements (
    achievement_id   INT IDENTITY(1,1) PRIMARY KEY,
    achievement_name CHAR(30) NOT NULL,
    description      CHAR(100),
    achievement_type NVARCHAR(100) NOT NULL,
    points           INT NOT NULL CHECK (points >= 0),
    icon_url         NVARCHAR(255) CHECK (
                        icon_url LIKE '%.svg' OR
                        icon_url LIKE '%.png' OR
                        icon_url LIKE '%.jpg'
                      )
);
GO

---------------------------------------------------------------------------------------------------
-- User Achievements
---------------------------------------------------------------------------------------------------
CREATE TABLE UserAchievements (
    user_id        INT NOT NULL,
    achievement_id INT NOT NULL,
    unlocked_at    DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (user_id, achievement_id),
    CONSTRAINT FK_UserAchievements_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE 
        ON UPDATE CASCADE,
    CONSTRAINT FK_UserAchievements_Achievements FOREIGN KEY (achievement_id) 
        REFERENCES Achievements(achievement_id) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Sold Games
---------------------------------------------------------------------------------------------------
CREATE TABLE SoldGames (
    sold_game_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id      INT NOT NULL,
    game_id      INT,
    sold_date    DATETIME,
    CONSTRAINT FK_SoldGames_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Reviews Given (mock)
---------------------------------------------------------------------------------------------------
CREATE TABLE ReviewsGiven (
    review_id   INT IDENTITY(1,1) PRIMARY KEY,
    user_id     INT NOT NULL,
    review_text NVARCHAR(MAX),
    review_date DATETIME,
    CONSTRAINT FK_ReviewsGiven_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Reviews Received (mock)
---------------------------------------------------------------------------------------------------
CREATE TABLE ReviewsReceived (
    review_id      INT IDENTITY(1,1) PRIMARY KEY,
    user_id        INT NOT NULL,
    review_comment NVARCHAR(MAX),
    review_date    DATETIME,
    CONSTRAINT FK_ReviewsReceived_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Posts (mock)
---------------------------------------------------------------------------------------------------
CREATE TABLE Posts (
    post_id     INT IDENTITY(1,1) PRIMARY KEY,
    user_id     INT NOT NULL,
    post_title  NVARCHAR(MAX),
    post_content NVARCHAR(MAX),
    CONSTRAINT FK_Posts_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

---------------------------------------------------------------------------------------------------
-- Reviews
---------------------------------------------------------------------------------------------------
CREATE TABLE Reviews (  
   ReviewId INT PRIMARY KEY IDENTITY(1,1), 
   Title NVARCHAR(100) NOT NULL, 
   Content NVARCHAR(2000) NOT NULL, 
   IsRecommended BIT, 
   Rating DECIMAL(3,1) CHECK (Rating BETWEEN 1.0 AND 5.0), 
   HelpfulVotes INT DEFAULT 0, 
   FunnyVotes INT DEFAULT 0, 
   HoursPlayed INT, 
   CreatedAt DATETIME DEFAULT GETDATE(), 
   UserId INT NOT NULL, 
   GameId INT NOT NULL, 
   CONSTRAINT FK_Review_User FOREIGN KEY (UserId) REFERENCES ReviewsUsers(UserId), 
   CONSTRAINT FK_Review_Game FOREIGN KEY (GameId) REFERENCES Games(GameId)
);
GO

---------------------------------------------------------------------------------------------------
-- Chat invites - Fixed to use renamed table
---------------------------------------------------------------------------------------------------
CREATE TABLE CHAT_INVITES(
    sender INT FOREIGN KEY REFERENCES ChatUsers(userid),
    receiver INT FOREIGN KEY REFERENCES ChatUsers(userid)
);
GO

---------------------------------------------------------------------------------------------------
-- Friend requests and relationships (alternative system)
---------------------------------------------------------------------------------------------------
CREATE TABLE FriendRequests (
    RequestId INT IDENTITY(1,1) PRIMARY KEY,
    SenderUsername NVARCHAR(50) NOT NULL,
    SenderEmail NVARCHAR(100) NOT NULL,
    SenderProfilePhotoPath NVARCHAR(255) NULL,
    ReceiverUsername NVARCHAR(50) NOT NULL,
    RequestDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_SenderReceiver UNIQUE (SenderUsername, ReceiverUsername)
);
GO

CREATE TABLE Friends (
    FriendshipId INT IDENTITY(1,1) PRIMARY KEY,
    User1Username NVARCHAR(50) NOT NULL,
    User2Username NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Friendship_Alt UNIQUE (User1Username, User2Username)
);
GO

---------------------------------------------------------------------------------------------------
-- Forum-related tables 
---------------------------------------------------------------------------------------------------
CREATE TABLE ForumPosts (
    post_id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(MAX),
    body NVARCHAR(MAX),
    creation_date DATETIME,
    author_id INT, -- should be foreign key referencing users table
    score INT,
    game_id INT NULL, -- should be foreign key referencing games table
);
GO

CREATE TABLE ForumComments (
    comment_id INT PRIMARY KEY IDENTITY(1,1),
    body NVARCHAR(MAX),
    creation_date DATETIME,
    author_id INT,
    score INT,
    post_id INT FOREIGN KEY REFERENCES ForumPosts(post_id) ON DELETE CASCADE
);
GO

CREATE TABLE UserLikedComment (
    userId INT, -- should be foreign key referencing users table
    comment_id INT FOREIGN KEY REFERENCES ForumComments(comment_id) ON DELETE CASCADE,
    PRIMARY KEY (userId, comment_id)
);
GO

CREATE TABLE UserLikedPost (
    userId INT, -- should be foreign key referencing users table
    post_id INT FOREIGN KEY REFERENCES ForumPosts(post_id) ON DELETE CASCADE,
    PRIMARY KEY (userId, post_id)
);
GO

CREATE TABLE UserDislikedComment (
    userId INT, -- should be foreign key referencing users table
    comment_id INT FOREIGN KEY REFERENCES ForumComments(comment_id) ON DELETE CASCADE,
    PRIMARY KEY (userId, comment_id)
);
GO

CREATE TABLE UserDislikedPost (
    userId INT, -- should be foreign key referencing users table
    post_id INT FOREIGN KEY REFERENCES ForumPosts(post_id) ON DELETE CASCADE,
    PRIMARY KEY (userId, post_id)
);
GO

---------------------------------------------------------------------------------------------------
-- News Posts and related tables
---------------------------------------------------------------------------------------------------
CREATE TABLE NewsPosts (
    pid INT PRIMARY KEY IDENTITY(1, 1),
    authorId INT,
    content NVARCHAR(MAX),
    uploadDate DATETIME,
    nrLikes INT,
    nrDislikes INT,
    nrComments INT
);
GO

CREATE TABLE NewsComments (
    cid INT PRIMARY KEY IDENTITY(1, 1),
    authorId INT,
    postId INT NULL FOREIGN KEY REFERENCES NewsPosts(pid) ON DELETE CASCADE,
    content NVARCHAR(MAX),
    uploadDate DATETIME,
);
GO

CREATE TABLE NewsRatings (
    postId INT FOREIGN KEY REFERENCES NewsPosts(pid) ON DELETE CASCADE,
    authorId INT,
    ratingType BIT,
    PRIMARY KEY(postId, authorId)
);
GO

CREATE TABLE ChatConversations (
        conversation_id INT PRIMARY KEY IDENTITY(1,1),
        user1_id INT NOT NULL,
        user2_id INT NOT NULL
);
GO

CREATE TABLE ChatMessages (
        message_id INT PRIMARY KEY IDENTITY(1,1),
        conversation_id INT NOT NULL,
	    sender_id INT NOT NULL,
	    timestamp BIGINT NOT NULL,
        message_format NVARCHAR(50) NOT NULL,
        message_content NVARCHAR(MAX) NOT NULL,
);
GO

---------------------------------------------------------------------------------------------------
-- Data insertions
---------------------------------------------------------------------------------------------------
-- Insert Users
INSERT INTO Users (email, username, hashed_password, developer, last_login) VALUES
    ('alice@example.com', 'AliceGamer', 'hashed_password_1', 1, '2025-03-20 14:25:00'),
    ('bob@example.com', 'BobTheBuilder', 'hashed_password_2', 0, '2025-03-21 10:12:00'),
    ('charlie@example.com','CharlieX', 'hashed_password_3', 0, '2025-03-22 18:45:00'),
    ('diana@example.com', 'DianaRocks', 'hashed_password_4', 0, '2025-03-19 22:30:00'),
    ('eve@example.com', 'Eve99', 'hashed_password_5', 1, '2025-03-23 08:05:00'),
    ('frank@example.com', 'FrankTheTank', 'hashed_password_6', 0, '2025-03-24 16:20:00'),
    ('grace@example.com', 'GraceSpeed', 'hashed_password_7', 0, '2025-03-25 11:40:00'),
    ('harry@example.com', 'HarryWizard', 'hashed_password_8', 0, '2025-03-20 20:15:00'),
    ('ivy@example.com', 'IvyNinja', 'hashed_password_9', 0, '2025-03-22 09:30:00'),
    ('jack@example.com', 'JackHacks', 'hashed_password_10',1, '2025-03-24 23:55:00'),
    ('user11@example.com','UserEleven', 'hashed_password_11',0, GETDATE()),
    ('user12@example.com','UserTwelve', 'hashed_password_12',0, GETDATE()),
    ('user13@example.com','UserThirteen', 'hashed_password_13',0, GETDATE());
GO

-- Insert into UserProfiles table
INSERT INTO UserProfiles (user_id, profile_picture,               bio,                         last_modified) VALUES
    (  1, 'ms-appx:///Assets/Collections/image.jpg',   'Gaming enthusiast and software developer', GETDATE()),
    (  2, 'ms-appx:///Assets/download.jpg',            'Game developer and tech lover',             GETDATE()),
    (  3, 'ms-appx:///Assets/download.jpg',            'Casual gamer and streamer',                 GETDATE()),
    (  4, 'ms-appx:///Assets/Collections/image.jpg',   'Casual gamer and streamer',                 GETDATE()),
    (  5, 'ms-appx:///Assets/download.jpg',            'Casual gamer and streamer',                 GETDATE()),
    (  6, 'ms-appx:///Assets/default_picture.jpg',     'Casual gamer and streamer',                 GETDATE()),
    (  7, 'ms-appx:///Assets/default_picture.jpg',     'Casual gamer and streamer',                 GETDATE()),
    (  8, 'ms-appx:///Assets/default_picture.jpg',     'Casual gamer and streamer',                 GETDATE()),
    (  9, 'ms-appx:///Assets/default_picture.jpg',     'Casual gamer and streamer',                 GETDATE()),
    ( 10, 'ms-appx:///Assets/default_picture.jpg',     'Casual gamer and streamer',                 GETDATE()),
    ( 11, 'ms-appx:///Assets/default_picture.jpg',     'Welcome new user!',                          GETDATE()),
    ( 12, 'ms-appx:///Assets/default_picture.jpg',     'Welcome new user!',                          GETDATE()),
    ( 13, 'ms-appx:///Assets/default_picture.jpg',     'Welcome new user!',                          GETDATE());
GO

-- Insert into ReviewsUsers table
INSERT INTO ReviewsUsers (UserId, Name, ProfilePicture) VALUES 
(2, 'Sam Carter', NULL),
(3, 'Taylor Kim', NULL);

-- Insert into Games table
INSERT INTO Games (GameId, Title, Description, ReleaseDate, CoverImage) VALUES
(0, 'Eternal Odyssey', 'An epic space exploration game.', '2022-11-05', NULL),
(1, 'Shadow Relic', 'A mystical action RPG set in ancient ruins.', '2021-06-14', NULL),
(2, 'Pixel Racers', 'Fast-paced arcade racing game.', '2023-02-20', NULL);

-- Insert Reviews table
INSERT INTO Reviews (Title, Content, IsRecommended, Rating, HelpfulVotes, FunnyVotes, HoursPlayed, CreatedAt, UserId, GameId) VALUES
('Loved it!', 'This game was amazing. I spent so many hours exploring!', 1, 4.8, 3, 1, 35, GETDATE(), 3, 0),
('Too buggy at launch', 'Crashes often but has potential.', 0, 2.5, 5, 0, 10, GETDATE(), 2, 1),
('Solid gameplay', 'It reminds me of old-school racers with a twist.', 1, 4.2, 2, 3, 20, GETDATE(), 3, 2),
('Good but needs polish', 'Fun mechanics but graphics are a bit outdated.', 1, 3.9, 1, 0, 18, GETDATE(), 2, 1),
('Underrated gem', 'A surprisingly deep and rewarding experience.', 1, 4.5, 4, 2, 40, GETDATE(), 2, 0);

-- Insert into ChatUsers table (renamed from USERS)
INSERT INTO ChatUsers (userid, username, ipAddress) VALUES 
    (1, 'JaneSmith', '192.168.50.247'),
    (2, 'Justin', '192.168.50.163'),
    (3, 'Andrei', '192.168.50.164'),
    (4, 'Marius', '192.168.50.165');
GO

-- Insert into FriendUsers
INSERT INTO FriendUsers (UserId, Username, Email, ProfilePhotoPath) VALUES
    (1, 'JaneSmith', 'jane.smith.69@fake.email.ai.com', 'ms-appx:///Assets/default_avatar.png'),
    (2, 'User1', 'user1@example.com', 'ms-appx:///Assets/friend1_avatar.png'),
    (3, 'User2', 'user2@example.com', 'ms-appx:///Assets/friend2_avatar.png'),
    (4, 'User3', 'user3@example.com', 'ms-appx:///Assets/friend3_avatar.png'),
    (5, 'User4', 'user4@example.com', 'ms-appx:///Assets/request1_avatar.png'),
    (6, 'User5', 'user5@example.com', 'ms-appx:///Assets/request2_avatar.png');
GO

-- Insert Friend Requests
INSERT INTO FriendRequests (SenderUsername, SenderEmail, SenderProfilePhotoPath, ReceiverUsername, RequestDate) VALUES 
    ('User4', 'user4@example.com', 'ms-appx:///Assets/request1_avatar.png', 'JaneSmith', DATEADD(day, -1, GETDATE())),
    ('User5', 'user5@example.com', 'ms-appx:///Assets/request2_avatar.png', 'JaneSmith', DATEADD(day, -2, GETDATE()));
GO

-- Insert Friends
INSERT INTO Friends (User1Username, User2Username) VALUES
    ('JaneSmith', 'User1'),
    ('JaneSmith', 'User2'),
    ('JaneSmith', 'User3');
GO

-- Points offers
INSERT INTO PointsOffers (numberOfPoints, value) VALUES
    (5, 2),
    (25, 8),
    (50, 15),
    (100, 20),
    (500, 50);
GO

-- Features
INSERT INTO Features (name, value, description, type, source) VALUES
    ('Black Hat', 2000, 'An elegant hat', 'hat', 'Assets/Features/Hats/black-hat.png'),
    ('Pufu', 10, 'Cute doggo', 'pet', 'Assets/Features/Pets/dog.png'),
    ('Kitty', 8, 'Cute cat', 'pet', 'Assets/Features/Pets/cat.png'),
    ('Frame', 5, 'Violet frame', 'frame', 'Assets/Features/Frames/frame1.png'),
    ('Love Emoji', 7, 'lalal', 'emoji', 'Assets/Features/Emojis/love.png'),
    ('Violet Background', 7, 'Violet Background', 'background', 'Assets/Features/Backgrounds/violet.jpg');
GO

-- Insert wallets
INSERT INTO Wallet (user_id, points, money_for_games) VALUES
    (1, 10, 200),
    (2, 10, 200),
    (3, 10, 200),
    (4, 10, 200),
    (5, 10, 200),
    (6, 10, 200),
    (7, 10, 200),
    (8, 10, 200),
    (9, 10, 200),
    (10, 10, 200),
    (11, 10, 200),
    (12, 10, 200),
    (13, 10, 200);
GO

INSERT INTO Collections (user_id, name,           cover_picture,                is_public, created_at) VALUES
    ( 1, 'All Owned Games', '/Assets/Collections/allgames.jpg',  1, '2022-02-21'),
    ( 1, 'Sports',         '/Assets/Collections/sports.jpg',    1, '2023-03-21'),
    ( 1, 'Chill Games',    '/Assets/Collections/chill.jpg',     1, '2024-03-21'),
    ( 1, 'X-Mas',          '/Assets/Collections/xmas.jpg',      0, '2025-02-21'),
    ( 2, 'Shooters',       '/Assets/Collections/shooters.jpg',  1, '2025-03-21'),
    ( 2, 'Pets',           '/Assets/Collections/pets.jpg',      0, '2025-01-21'),
    ( 11,'All Owned Games', '/Assets/Collections/allgames.jpg',  1, '2022-02-21'),
    ( 11,'Shooters',        '/Assets/Collections/shooters.jpg',  1, '2025-03-21'),
    ( 11,'Sports',          '/Assets/Collections/sports.jpg',    1, '2023-03-21'),
    ( 11,'Chill Games',     '/Assets/Collections/chill.jpg',     1, '2024-03-21'),
    ( 11,'Pets',            '/Assets/Collections/pets.jpg',      0, '2025-01-21'),
    ( 11,'X-Mas',           '/Assets/Collections/xmas.jpg',      0, '2025-02-21');
GO

INSERT INTO OwnedGames (user_id, title,                    description,                   cover_picture) VALUES
    (11, 'Call of Duty: MWIII',  'First?person military shooter',    '/Assets/Games/codmw3.png'),
    (11, 'Overwatch2',          'Team?based hero shooter',           '/Assets/Games/overwatch2.png'),
    (11, 'Counter?Strike2',     'Tactical shooter',                  '/Assets/Games/cs2.png'),
    (11, 'FIFA25',              'Football simulation',               '/Assets/Games/fifa25.png'),
    (11, 'NBA2K25',             'Basketball simulation',             '/Assets/Games/nba2k25.png'),
    (11, 'Tony Hawk Pro Skater', 'Skateboarding sports game',         '/Assets/Games/thps.png'),
    (11, 'Stardew Valley',       'Relaxing farming game',             '/Assets/Games/stardewvalley.png'),
    (11, 'The Sims4: Cats & Dogs','Life sim with pets',               '/Assets/Games/sims4pets.png'),
    (11, 'Nintendogs',           'Pet care simulation',               '/Assets/Games/nintendogs.png'),
    (11, 'Pet Hotel',            'Manage a hotel for pets',           '/Assets/Games/pethotel.png'),
    (11, 'Christmas Wonderland','Festive hidden object game',        '/Assets/Games/xmas.png');
GO

ALTER TABLE Features
ADD equipped BIT NOT NULL DEFAULT 0;

--ADD ACHIEVEMENTS TO DB
GO
INSERT INTO Achievements (achievement_name, description,                achievement_type,               points) VALUES
    ('FRIENDSHIP1',  'You made a friend, you get a point',             'Friendships',                  1),
    ('FRIENDSHIP2',  'You made 5 friends, you get 3 points',           'Friendships',                  3),
    ('FRIENDSHIP3',  'You made 10 friends, you get 5 points',          'Friendships',                  5),
    ('FRIENDSHIP4',  'You made 50 friends, you get 10 points',         'Friendships',                  10),
    ('FRIENDSHIP5',  'You made 100 friends, you get 15 points',        'Friendships',                  15),
    ('OWNEDGAMES1',  'You own 1 game, you get 1 point',                'Owned Games',                  1),
    ('OWNEDGAMES2',  'You own 5 games, you get 3 points',              'Owned Games',                  3),
    ('OWNEDGAMES3',  'You own 10 games, you get 5 points',             'Owned Games',                  5),
    ('OWNEDGAMES4',  'You own 50 games, you get 10 points',            'Owned Games',                  10),
    ('SOLDGAMES1',   'You sold 1 game, you get 1 point',               'Sold Games',                   1),
    ('SOLDGAMES2',   'You sold 5 games, you get 3 points',             'Sold Games',                   3),
    ('SOLDGAMES3',   'You sold 10 games, you get 5 points',            'Sold Games',                   5),
    ('SOLDGAMES4',   'You sold 50 games, you get 10 points',           'Sold Games',                   10),
    ('REVIEW1',      'You gave 1 review, you get 1 point',            'Number of Reviews Given',      1),
    ('REVIEW2',      'You gave 5 reviews, you get 3 points',          'Number of Reviews Given',      3),
    ('REVIEW3',      'You gave 10 reviews, you get 5 points',         'Number of Reviews Given',      5),
    ('REVIEW4',      'You gave 50 reviews, you get 10 points',        'Number of Reviews Given',      10),
    ('REVIEWR1',     'You got 1 review, you get 1 point',             'Number of Reviews Received',   1),
    ('REVIEWR2',     'You got 5 reviews, you get 3 points',           'Number of Reviews Received',   3),
    ('REVIEWR3',     'You got 10 reviews, you get 5 points',          'Number of Reviews Received',   5),
    ('REVIEWR4',     'You got 50 reviews, you get 10 points',         'Number of Reviews Received',   10),
    ('DEVELOPER',    'You are a developer, you get 10 points',        'Developer',                    10),
    ('ACTIVITY1',    'You have been active for 1 year, you get 1 point','Years of Activity',            1),
    ('ACTIVITY2',    'You have been active for 2 years, you get 3 points','Years of Activity',            3),
    ('ACTIVITY3',    'You have been active for 3 years, you get 5 points','Years of Activity',            5),
    ('ACTIVITY4',    'You have been active for 4 years, you get 10 points','Years of Activity',            10),
    ('POSTS1',       'You have made 1 post, you get 1 point',          'Number of Posts',              1),
    ('POSTS2',       'You have made 5 posts, you get 3 points',        'Number of Posts',              3),
    ('POSTS3',       'You have made 10 posts, you get 5 points',       'Number of Posts',              5),
    ('POSTS4',       'You have made 50 posts, you get 10 points',      'Number of Posts',              10);
GO

-- Add a generic icon URL to all achievements
UPDATE Achievements
SET icon_url = 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'
WHERE achievement_id > 0;
GO

select * from Users
select * from UserProfiles


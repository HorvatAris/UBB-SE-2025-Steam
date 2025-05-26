USE FinalSteamDB;
GO

---------------------------------------------------------------------------------------------------
-- Users
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
    profile_picture  NVARCHAR(255) CHECK (
                       profile_picture LIKE '%.svg' OR
                       profile_picture LIKE '%.png' OR
                       profile_picture LIKE '%.jpg'
                     ),
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
-- Owned Games Collection Link
---------------------------------------------------------------------------------------------------
CREATE TABLE OwnedGames_Collection (
    collection_id INT NOT NULL,
    game_id       INT NOT NULL,
    PRIMARY KEY (collection_id, game_id),
    CONSTRAINT FK_OGC_Collections FOREIGN KEY (collection_id)
        REFERENCES Collections(collection_id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_OGC_OwnedGames  FOREIGN KEY (game_id)
        REFERENCES OwnedGames(game_id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
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
    CONSTRAINT FK_FeatureUser_Users    FOREIGN KEY (user_id)    REFERENCES Users(user_id)    ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_FeatureUser_Features FOREIGN KEY (feature_id) REFERENCES Features(feature_id) ON DELETE CASCADE ON UPDATE CASCADE
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
    CONSTRAINT FK_UserAchievements_Users        FOREIGN KEY (user_id)        REFERENCES Users(user_id)        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_UserAchievements_Achievements FOREIGN KEY (achievement_id) REFERENCES Achievements(achievement_id) ON DELETE CASCADE ON UPDATE CASCADE
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

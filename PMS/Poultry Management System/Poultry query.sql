CREATE TABLE Scategory(
    [id]   INT IDENTITY(1, 1) primary key not null,
    [category] NVARCHAR (50) ,
  
);

CREATE TABLE expensecategory(
    [Id]           INT            IDENTITY(1, 1) NOT NULL,
    [category] NVARCHAR (50)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



create table dcategory(
id int identity(100,1) primary key,
category nvarchar(50),
);

create table deathpoultry(
did int identity (100,1) primary key,
category nvarchar(50) ,
deathdate datetime,
causeofdeath nvarchar(MAX),
);


create table Expense(
Id int identity (100,1) primary key,
[expense name] nvarchar(50) not null,
category nvarchar(50) not null,
amount int not null,
[date] datetime not null,
[description] nvarchar(MAX) not null,
);

create table login(
cid int identity(100,1) primary key,
email nvarchar(50) not null,
password nvarchar(50) not null,
type nvarchar(50),
);


create table Sales(
id int identity(100,1) primary key,
sales nvarchar(50) not null,
category nvarchar(50) not null,
price int not null,
date datetime not null,
description nvarchar(50) not null,
);


create database PrimaryDb;
go

create database SecondaryDb;
go

use PrimaryDb;
go
create table PrimaryTable
(
    Id   int identity
        constraint PrimaryTable_pk
            primary key,
    Name VARCHAR(50) not null
)
go

use SecondaryDb
go
create table SecondaryTable
(
    Id   int identity
        constraint SecondaryTable_pk
            primary key,
    Name VARCHAR(50) not null
)
go



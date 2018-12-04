CREATE TABLE [dbo].[Test1]
(
    [Id] int IDENTITY(1,1) PRIMARY KEY,
    [Guid] uniqueidentifier NOT NULL,
    [NGuid] uniqueidentifier,
    [FixedText] nchar(255) NOT NULL,
    [NFixedText] nchar(255),
    [SmallText] nvarchar(255) NOT NULL,
    [NSmallText] nvarchar(255),
    [Text] ntext NOT NULL,
    [NText] ntext,
    [Bool] bit NOT NULL,
    [NBool] bit,
    [Int8] tinyint NOT NULL,
    [NInt8] tinyint,
    [Int16] smallint NOT NULL,
    [NInt16] smallint,
    [Int32] int NOT NULL,
    [NInt32] int,
    [Int64] bigint NOT NULL,
    [NInt64] bigint,
    [Date] date NOT NULL,
    [NDate] date,
    [Time] time NOT NULL,
    [NTime] time,
    [DateTime] datetime2 NOT NULL,
    [NDateTime] datetime2,
    [Decimal] numeric(15,2) NOT NULL,
    [NDecimal] numeric(15,2),
    [Float] float(4) NOT NULL,
    [NFloat] float(4),
    [Double] float(8) NOT NULL,
    [NDouble] float(8)
);

insert into [dbo].[Test1] (
[Guid],
[NGuid],
[FixedText],
[NFixedText],
[SmallText],
[NSmallText],
[Text],
[NText],
[Bool],
[NBool],
[Int8],
[NInt8],
[Int16],
[NInt16],
[Int32],
[NInt32],
[Int64],
[NInt64],
[Float],
[NFloat],
[Double],
[NDouble],
[Decimal],
[NDecimal],
[Date],
[NDate],
[Time],
[NTime],
[DateTime],
[NDateTime]
)
values
(
'7170fe83-7925-4970-aa61-62da119a09a0',--Guid
'7170fe83-7925-4970-aa61-62da119a09a0',--NGuid
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--FixedText
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--NFixedText
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--SmallText
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--NSmallText
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--Text
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--NText
'true',--Bool
'true',--NBool
10,--Int8
11,--NInt8
12,--Int16
13,--NInt16
14,--Int32
15,--NInt32
16,--Int64
17,--NInt64
18.11,--Float
19.22,--NFloat
20.33,--Double
21.44,--NDouble
22.55,--Decimal
23.66,--NDecimal
'2018-07-15',--Date
'2018-07-15',--NDate
'12:34:56',--Time
'12:34:56',--NTime
'2018-07-15 12:34:56',--DateTime
'2018-07-15 12:34:56'--NDateTime
),
(
'7170fe83-7925-4970-aa61-62da119a09a0',--Guid
NULL,--NGuid
'',--FixedText
'',--NFixedText
'',--SmallText
'',--NSmallText
'',--Text
'',--NText
'false',--Bool
'false',--NBool
10,--Int8
NULL,--NInt8
12,--Int16
NULL,--NInt16
14,--Int32
NULL,--NInt32
16,--Int64
NULL,--NInt64
18.11,--Float
NULL,--NFloat
20.33,--Double
NULL,--NDouble
22.55,--Decimal
NULL,--NDecimal
'2018-07-15',--Date
NULL,--NDate
'12:34:56',--Time
NULL,--NTime
'2018-07-15 12:34:56',--DateTime
NULL--NDateTime
),
(
'7170fe83-7925-4970-aa61-62da119a09a0',--Guid
NULL,--NGuid
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--FixedText
NULL,--NFixedText
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--SmallText
NULL,--NSmallText
replace(replace(replace('Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"''`\r\n\tadipisicing\r\nelit.', '\r', char(13)), '\n', char(10)), '\t', char(09)),--Text
NULL,--NText
'false',--Bool
NULL,--NBool
10,--Int8
NULL,--NInt8
12,--Int16
NULL,--NInt16
14,--Int32
NULL,--NInt32
16,--Int64
NULL,--NInt64
18.11,--Float
NULL,--NFloat
20.33,--Double
NULL,--NDouble
22.55,--Decimal
NULL,--NDecimal
'2018-07-15',--Date
NULL,--NDate
'12:34:56',--Time
NULL,--NTime
'2018-07-15 12:34:56',--DateTime
NULL--NDateTime
);
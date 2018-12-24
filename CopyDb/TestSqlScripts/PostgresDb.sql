CREATE TABLE "public"."Test1"
(
    "Id" serial NOT NULL,
    "Guid" uuid NOT NULL,
    "NGuid" uuid,
    "FixedText" character(255) NOT NULL,
    "NFixedText" character(255),
    "SmallText" character varying(255) NOT NULL,
    "NSmallText" character varying(255),
    "Text" text NOT NULL,
    "NText" text,
    "Bool" boolean NOT NULL,
    "NBool" boolean,
    "Int8" smallint NOT NULL,
    "NInt8" smallint,
    "Int16" smallint NOT NULL,
    "NInt16" smallint,
    "Int32" integer NOT NULL,
    "NInt32" integer,
    "Int64" bigint NOT NULL,
    "NInt64" bigint,
    "Date" date NOT NULL,
    "NDate" date,
    "Time" time NOT NULL,
    "NTime" time,
    "DateTime" timestamp NOT NULL,
    "NDateTime" timestamp,
    "Decimal" numeric(15,2) NOT NULL,
    "NDecimal" numeric(15,2),
    "Float" real NOT NULL,
    "NFloat" real,
    "Double" double precision NOT NULL,
    "NDouble" double precision,
    PRIMARY KEY ("Id")
);

CREATE TABLE "public"."Test1"
(
    "Id" serial NOT NULL,
    "Guid" uuid NOT NULL,
    "NGuid" uuid,
    "FixedText" character(255) NOT NULL,
    "NFixedText" character(255),
    "SmallText" character varying(255) NOT NULL,
    "NSmallText" character varying(255),
    "Text" text NOT NULL,
    "NText" text,
    "Bool" boolean NOT NULL,
    "NBool" boolean,
    "Int8" smallint NOT NULL,
    "NInt8" smallint,
    "Int16" smallint NOT NULL,
    "NInt16" smallint,
    "Int32" integer NOT NULL,
    "NInt32" integer,
    "Int64" bigint NOT NULL,
    "NInt64" bigint,
    "Date" date NOT NULL,
    "NDate" date,
    "Time" time NOT NULL,
    "NTime" time,
    "DateTime" timestamp NOT NULL,
    "NDateTime" timestamp,
    "Decimal" numeric(15,2) NOT NULL,
    "NDecimal" numeric(15,2),
    "Float" real NOT NULL,
    "NFloat" real,
    "Double" double precision NOT NULL,
    "NDouble" double precision,
    "ConstFixedText" character(255) NOT NULL,
    "ConstSmallText" character varying(255) NOT NULL,
    "ConstText" text NOT NULL,
    "ConstBool" boolean NOT NULL,
    "ConstInt8" smallint NOT NULL,
    "ConstInt16" smallint NOT NULL,
    "ConstInt32" integer NOT NULL,
    "ConstInt64" bigint NOT NULL,
    "ConstDate" date NOT NULL,
    "ConstTime" time NOT NULL,
    "ConstDateTime" timestamp NOT NULL,
    "ConstDecimal" numeric(15,2) NOT NULL,
    "ConstFloat" real NOT NULL,
    "ConstDouble" double precision NOT NULL,
    PRIMARY KEY ("Id")
);

insert into "public"."Test1" (
"Guid",
"NGuid",
"FixedText",
"NFixedText",
"SmallText",
"NSmallText",
"Text",
"NText",
"Bool",
"NBool",
"Int8",
"NInt8",
"Int16",
"NInt16",
"Int32",
"NInt32",
"Int64",
"NInt64",
"Float",
"NFloat",
"Double",
"NDouble",
"Decimal",
"NDecimal",
"Date",
"NDate",
"Time",
"NTime",
"DateTime",
"NDateTime"
)
values
(
'7170fe83-7925-4970-aa61-62da119a09a0',--Guid
'7170fe83-7925-4970-aa61-62da119a09a0',--NGuid
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--FixedText
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--NFixedText
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--SmallText
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--NSmallText
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--Text
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--NText
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
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--FixedText
NULL,--NFixedText
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--SmallText
NULL,--NSmallText
E'Lorem \r\n\t 01234567890\r\n\tipsum dolor\r\n\t()<>{}[]|\/|?.,+-=!@#$%^&*"\'`\r\n\tadipisicing\r\nelit.',--Text
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
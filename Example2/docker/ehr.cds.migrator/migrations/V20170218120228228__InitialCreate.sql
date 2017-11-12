CREATE TABLE "public"."composition" ("id" varchar(100) NOT NULL, "claim_id" varchar(100) NOT NULL, "episodeofcare_id" varchar(100) NOT NULL, "subject_id" varchar(100) NOT NULL, "status" varchar(100) NOT NULL, "last_updated" timestamp NOT NULL, CONSTRAINT "PK_composition" PRIMARY KEY ("id"));

CREATE TABLE "public"."episodeofcare" ("id" varchar(100) NOT NULL, "start" timestamp NOT NULL, "end" timestamp NOT NULL, "patient_id" varchar(100) NOT NULL, "status" varchar(100) NOT NULL, "last_updated" timestamp NOT NULL, CONSTRAINT "PK_episodeofcare" PRIMARY KEY ("id"));

CREATE TABLE "public"."episodeofcare_condition" ("episodeofcare_id" varchar(100) NOT NULL, "condition_id" varchar(100) NOT NULL);


CREATE TABLE "public"."condition" ("id" varchar(100) NOT NULL, "diagnosis_code" varchar(10) NOT NULL, "status" varchar(100) NOT NULL, "last_updated" timestamp NOT NULL, CONSTRAINT "PK_condition" PRIMARY KEY ("id"));

CREATE TABLE "public"."patient" ("id" varchar(100) NOT NULL, "date_birth" timestamp NULL, "last_updated" timestamp NOT NULL, CONSTRAINT "PK_patient" PRIMARY KEY ("id"));

CREATE INDEX "IX_patient_id" ON "public"."episodeofcare" ("patient_id" ASC);
CREATE INDEX "IX_subject_id" ON "public"."composition" ("subject_id" ASC);
CREATE INDEX "IX_episodeofcare_id" ON "public"."episodeofcare" ("id" ASC);
CREATE INDEX "IX_patient_index" ON "public"."patient" ("id" ASC);

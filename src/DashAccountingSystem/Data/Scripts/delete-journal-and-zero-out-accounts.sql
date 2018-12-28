/* DANGER - Don't do it unless you mean it! */
/* Affects all tenants!                     */

TRUNCATE TABLE "AccountingPeriodClosingBalance";

UPDATE "Account"
   SET "PendingDebits" = NULL
      ,"PendingCredits" = NULL
      ,"CurrentBalance" = 0
      ,"BalanceUpdated" = now() AT TIME ZONE 'UTC';

TRUNCATE TABLE "JournalEntry" RESTART IDENTITY CASCADE;
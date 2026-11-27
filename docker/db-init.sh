#!/bin/bash
set -e

SQLCMD="/opt/mssql-tools18/bin/sqlcmd -S db -U sa -P $MSSQL_SA_PASSWORD -C -b"

EXISTS=$($SQLCMD -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = 'FPPR'" | tr -d '[:space:]')

if [ "$EXISTS" = "1" ]; then
    echo "FPPR already exists, skip."
    exit 0
fi

for f in DDL.sql StoredProcedure.sql DML.sql; do
    echo "Running $f"
    $SQLCMD -i "/db/$f"
done

echo "FPPR initialized."

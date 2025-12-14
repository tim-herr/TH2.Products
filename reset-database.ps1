# Reset Database Script
# This script deletes the SQLite database file so it will be recreated with fresh seed data

Write-Host "Resetting Products Database..." -ForegroundColor Cyan

$dbPath = "TH2.Products.API\products.db"
$dbShmPath = "TH2.Products.API\products.db-shm"
$dbWalPath = "TH2.Products.API\products.db-wal"

if (Test-Path $dbPath) {
    Remove-Item $dbPath -Force
    Write-Host "? Deleted products.db" -ForegroundColor Green
} else {
    Write-Host "? products.db not found (already deleted or doesn't exist)" -ForegroundColor Yellow
}

if (Test-Path $dbShmPath) {
    Remove-Item $dbShmPath -Force
    Write-Host "? Deleted products.db-shm" -ForegroundColor Green
}

if (Test-Path $dbWalPath) {
    Remove-Item $dbWalPath -Force
    Write-Host "? Deleted products.db-wal" -ForegroundColor Green
}

Write-Host "`nDatabase reset complete!" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Run your API (press F5)" -ForegroundColor White
Write-Host "2. The database will be recreated with updated seed data" -ForegroundColor White
Write-Host "3. Test the search: GET /api/products/search?searchTerm=winter" -ForegroundColor White

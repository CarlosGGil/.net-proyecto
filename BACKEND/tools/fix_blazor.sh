#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
CLIENT="$ROOT/src/Espectaculos.Blazor.Client"
APP="$ROOT/src/Espectaculos.Application"
INFRA="$ROOT/src/Espectaculos.Infrastructure"
API="$ROOT/src/Espectaculos.WebApi"

INDEX="$CLIENT/wwwroot/index.html"
if grep -q '</head>' "$INDEX"; then
  sed -i '' '/<\/head>/i\
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />\
<script src="_content/MudBlazor/MudBlazor.min.js"></script>\
<link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">\
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
' "$INDEX"
fi

for f in "$CLIENT"/Pages/*.razor; do
  sed -i '' 's/\bbind-Value=/@bind-Value=/g' "$f"
done

for f in "$CLIENT"/Pages/*.razor; do
  sed -i '' 's/<RowTemplate>/<RowTemplate Context="context">/g' "$f"
done

DET="$CLIENT/Pages/EventoDetalle.razor"
if [ -f "$DET" ]; then
  if ! grep -q '@inject NavigationManager' "$DET"; then
    sed -i '' '1s;^;@inject NavigationManager Nav\n;' "$DET"
  fi
  sed -i '' 's/NavigationManager\.NavigateTo/Nav.NavigateTo/g' "$DET"
  sed -i '' 's/<MudNumericField[^>]*Value=/<MudNumericField @bind-Value=/g' "$DET"
fi

EV="$CLIENT/Pages/Eventos.razor"
OR="$CLIENT/Pages/Ordenes.razor"
[ -f "$EV" ] && sed -i '' 's/<RowTemplate>/<RowTemplate Context="context">/g' "$EV"
[ -f "$OR" ] && sed -i '' 's/<RowTemplate>/<RowTemplate Context="context">/g' "$OR"

mkdir -p "$APP/Abstractions/Repositories"
cat > "$APP/Abstractions/Repositories/IEventoRepository.cs" <<'EOF'
using Espectaculos.Application.Common;
using Espectaculos.Application.DTOs;
using Espectaculos.Domain.Entities;

namespace Espectaculos.Application.Abstractions.Repositories;

public interface IEventoRepository
{
    Task<Evento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Evento entity, CancellationToken ct = default);
    void Update(Evento entity);

    Task<PagedResult<EventoDto>> SearchAsync(
        string? q, string? sort, string? dir,
        int page, int pageSize, bool onlyPublished,
        CancellationToken cancellationToken = default);
}
EOF

dotnet add "$INFRA" reference "$APP/Espectaculos.Application.csproj" || true

dotnet restore
dotnet build "$API" -c Debug
dotnet build "$CLIENT" -c Debug

# Tournois E-Sport API

API ASP.NET Core .NET 11 pour gérer des jeux, équipes, joueurs, tournois,
inscriptions et matchs.

## Architecture

Le projet utilise une architecture trois niveaux dans un projet unique :

- API : contrôleurs, authentification JWT et Swagger
- Business : DTOs et services métier
- Data Access : entités EF Core, `ApplicationContext`, repositories et Unit of Work

MediatR est utilisé pour les opérations CQRS sélectionnées.

## Prérequis

- .NET SDK 11
- SQL Server Express LocalDB ou SQL Server
- SQL Server accessible avec la chaîne configurée

## Configuration

La connexion SQL Server est configurée avec `ConnectionStrings:cnx`.
Pour éviter de stocker des secrets, surchargez les valeurs sensibles avec des
variables d'environnement :

```powershell
$env:JWT__SecretKey = "une-cle-secrete-longue-et-aleatoire"
$env:AdminSeed__Email = "admin@example.com"
$env:AdminSeed__Password = "MotDePasseAdminFort123!"
```

Les rôles `Admin`, `Organizer` et `Player` sont créés automatiquement.
Le compte administrateur n'est créé que si les deux variables `AdminSeed` sont
présentes.

## Base de données

```powershell
dotnet ef migrations add NomDeLaMigration
dotnet ef database update
```

Les migrations existantes sont dans [Migrations](./Migrations).

## Lancer l'API

```powershell
dotnet run --launch-profile https
```

- Swagger : `https://localhost:7007`
- API HTTP : `http://localhost:5192`

## Authentification

1. Appeler `POST /api/auth/register`.
2. Appeler `POST /api/auth/login`.
3. Copier le JWT retourné.
4. Utiliser le bouton **Authorize** de Swagger avec `Bearer <token>`.

## Principales routes

- `/api/auth/register`, `/api/auth/login`
- `/api/roles`
- `/api/games`
- `/api/teams`
- `/api/players`
- `/api/tournaments`
- `/api/tournament-registrations`
- `/api/matches`
- `/api/tournaments/{id}/standings`

Les collections utilisent `page` et `pageSize`, par exemple :
`GET /api/games?page=1&pageSize=10`.

## Tests manuels

Les requêtes principales sont disponibles dans [API.http](./API.http).

# SGIP — Sistema de Gestión de Inversiones y Prestamos

Sistema FinTech para simulación, solicitud y gestión de préstamos con procesamiento de transacciones de pagos y desembolsos. 

## Links de Despliegue

| Backend Api:https://fintech-gk1h.onrender.com/swagger/index.html
| Frontend: https://fintech-taupe-psi.vercel.app|
| Usuario de prueba: `user-001` |

## Tecnologías Utilizadas

### Backend
- .NET 8 — Web API
- PostgreSQL + Entity Framework Core 8 — persistencia
- Serilog — logging estructurado
- Swagger  — documentación 
- xUnit + Moq — testing

### Frontend
- Next.js 16 (App Router) + React 19 + TypeScript
- PrimeReact 10 — componentes UI
- Tailwind CSS 4 — utilidades de layout
- TanStack Query v5 — server state y cache
- React Hook Form + Zod v4 — formularios y validación
- Axios — cliente HTTP

### Despliegue
- **Render** — backend + PostgreSQL
- **Vercel** — frontend

## Instalación Local

### Prerrequisitos
- .NET 8 SDK
- Node.js 20+
- pnpm (`npm install -g pnpm`)
- PostgreSQL 14+

### Backend

bash
cd backend/FinTech

dotnet restore
dotnet ef database update --project Infraestructura/FinTech.Infrastructure --startup-project Presentacion/FinTech.Api
dotnet run --project Presentacion/FinTech.Api

Swagger disponible en: https://localhost:7133 o http://localhost:5270

### Frontend

bash
cd frontend
pnpm install
pnpm dev


App disponible en: http://localhost:3000


## Variables de Entorno

### Backend (Render env vars)

DATABASE_URL=postgresql:postgresql://fintech_db_69yf_user:EKe8h5iGztCOGUth2Bt25xcPlKPLZEqm@dpg-d84uas19rddc739tloo0-a/fintech_db_69yf
ASPNETCORE_ENVIRONMENT=Production


### Backend (local — `appsettings.Development.json`)
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fintech_db;Username=;Password="
  }
}

### Frontend (`.env.local`)

NEXT_PUBLIC_API_URL=http://localhost:5270

## Testing


cd backend/FinTech
dotnet test Tests/FinTech.Tests


**9 tests — todos en verde:**
- `CalculateFixedPayment_ReturnsCorrectAmount`
- `GenerateSchedule_ReturnsCorrectNumberOfPayments`
- `GenerateSchedule_LastBalanceIsZero`
- `CalculateTEM_FromTEA_ReturnsCorrectValue`
- `SimulateLoan_WithAmountBelowMinimum_ThrowsArgumentException`
- `SimulateLoan_WithAmountAboveMaximum_ThrowsArgumentException`
- `SimulateLoan_WithTermBelowMinimum_ThrowsArgumentException`
- `SimulateLoan_WithTermAboveMaximum_ThrowsArgumentException`
- `CreateTransaction_WithDuplicateIdempotencyKey_ReturnsSameTransaction`



## Arquitectura

### Backend — Clean Architecture (multi-proyecto)


FinTech/
├── Dominio/FinTech.Domain/           # Entidades, Enumeraciones, FinancialCalculator
├── Aplicacion/
│   ├── FinTech.Application.DTO/      # DTOs request/response
│   ├── FinTech.Application.Interfaces/ # ILoanRepository, ITransactionRepository, ILoanService
│   └── FinTech.Application.UseCases/ # LoanService, TransactionService
├── Infraestructura/FinTech.Infrastructure/
│   └── Persistencia/
│       ├── Configuraciones/          # EF Fluent API
│       ├── Migraciones/              # EF migrations versionadas
│       └── Repositorios/             # LoanRepository, TransactionRepository
└── Presentacion/FinTech.Api/
    ├── Controladores/                # LoansController, TransactionsController
    └── Middleware/                   # ExceptionMiddleware 


### Frontend — Feature-based 

frontend/
├── app/              # App Router — solo enrutamiento, sin lógica
│   ├── loans/
│   │   ├── simulate/page.tsx
│   │   ├── [id]/page.tsx
│   │   └── page.tsx
│   └── transactions/page.tsx
├── features/         # Módulos por dominio
│   ├── loans/
│   │   ├── components/   # LoanSimulator, PaymentScheduleTable, LoanList
│   │   ├── hooks/        # useLoans, useSimulateLoan, useCreateLoan
│   │   ├── services/     # loanService (llamadas API)
│   │   ├── types/        # LoanResponse, SimulateLoanRequest
│   │   └── pages/        # LoanListPage, LoanDetailPage, LoanSimulatePage
│   └── transactions/
│       ├── components/
│       ├── hooks/
│       ├── services/
│       ├── types/
│       └── pages/
└── shared/           # Compartido entre features
    ├── components/   # Navbar, StatusBadge
    └── lib/          # api.ts (axios)

### Patrones Implementados

| Patrón | Dónde | Por qué |

| Repository Pattern | `Application.Interfaces` + `Infrastructure/Repositorios` | Abstrae el acceso a datos. Los servicios no conocen EF Core. |
| Dependency Inversion (DIP) | `Program.cs` registra `ILoanRepository → LoanRepository` | Controllers dependen de interfaces, no de implementaciones. |
| Middleware de excepciones | `ExceptionMiddleware` | Centraliza manejo de errores en un único lugar (SRP). |
| Idempotency Key | `TransactionService.CreateTransactionAsync` | Garantiza que doble-click o retry no duplique transacciones. |
| Feature-based modules** | Frontend `features/` | Cada dominio es autónomo: cohesión alta, acoplamiento bajo. |


## Decisiones de Diseño

**¿Por qué Clean Architecture multi-proyecto en lugar de la estructura del enunciado?**
Separa las dependencias físicamente si mañana se cambia PostgreSQL por MongoDB, solo cambia capa de Infrastructure.

**¿Por qué TEA fija del 24%?**
Simplifica la UX (el usuario no elige tasa) y mantiene los cálculos verificables. El campo tea es pasable desde el request para facilitar tests.

**¿Por qué TanStack Query en lugar de useState + fetch?**
Cache automático, deduplicación de requests, tras mutaciones. Evita estados de loading/error manuales en cada componente.

**¿Por qué PrimeReact + Tailwind?**
PrimeReact provee componentes complejos listos ejemplo DataTable con scroll, Dropdown, InputNumber con formato de moneda. Tailwind maneja layout y espaciado y maquetado.

**¿Qué se simplificó?**
- Sin autenticación
- Solo se implemento el sistema francés (cuota fija)
- Se incluyo la Auto-aprobación automática: monto < $10,000 y < 2 préstamos activos

**Mejoras futuras:**
- JWT authentication
- Encryptacion
- Sistema alemán (cuota decreciente) con Strategy pattern
- Notificaciones en tiempo real (SignalR)
- Dashboard con métricas



## Supuestos y Limitaciones

- `userId` hardcodeado en frontend (`user-001`). En producción debería usarse token JWT y un metodo para el logeo inicial.
- En produccion real deberia estar encritpado cada llamada.
- TEA fija al 24% para simplificar la UX.
- Se ingluye data de prueba.
- Se incluyo el campo ingreso mensual y se lo volivio mandatorio esto al solicitar préstamo para validar el 40% de ingresos.



## Screenshots

<img width="1913" height="568" alt="image" src="https://github.com/user-attachments/assets/2f300398-70ff-46b7-bc2e-6873c42f92e3" />

<img width="1643" height="1036" alt="image" src="https://github.com/user-attachments/assets/b976f600-0daf-4974-899d-2cfef953e6c0" />

<img width="1785" height="397" alt="image" src="https://github.com/user-attachments/assets/7e1a7053-8100-43b2-99c2-12878b7f3b5f" />

<img width="1695" height="628" alt="image" src="https://github.com/user-attachments/assets/48206e84-5120-48d2-a31e-c903f2f6578f" />



![alt text](image-4.png)

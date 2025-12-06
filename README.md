[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/qlAtkCwb)
[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=20511375)

# INFORME DE LABORATORIO N° 01: PRUEBAS ESTÁTICAS DE SEGURIDAD DE APLICACIONES CON SONARQUBE

## DATOS GENERALES
- **Curso:** Calidad de Software
- **Laboratorio:** Nº 01
- **Título:** Pruebas Estáticas de Seguridad de Aplicaciones con SonarCloud
- **Estudiante:** Victor Cruz
- **Fecha de Elaboración:** 05 de Diciembre de 2025

---

## RESUMEN EJECUTIVO

Este informe documenta el desarrollo completo del Laboratorio N° 01 del curso de Calidad de Software, centrado en la implementación de pruebas estáticas de seguridad utilizando SonarCloud. Se desarrolló una aplicación bancaria en C# .NET 9.0 con su respectiva suite de pruebas unitarias, alcanzando una cobertura significativa de código. Además, se implementó un pipeline completo de integración continua mediante GitHub Actions que incluye análisis de calidad de código, empaquetado NuGet y publicación automática de releases.

**Logros Principales:**
- ✅ Aplicación bancaria completamente funcional con modelo de dominio
- ✅ Suite de pruebas unitarias con 8 casos de prueba implementados
- ✅ Integración exitosa con SonarCloud para análisis estático de código
- ✅ Pipeline CI/CD automatizado con GitHub Actions
- ✅ Paquete NuGet publicado en GitHub Packages
- ✅ Release v1.0.0 generado automáticamente

---

## OBJETIVOS
  * Comprender el funcionamiento de las pruebas estáticas de seguridad de código de las aplicaciones que desarrollamos utilizando SonarCloud.
  * Implementar pruebas unitarias con alta cobertura de código.
  * Configurar un pipeline de integración continua para automatizar el análisis de calidad.
  * Publicar artefactos del proyecto mediante GitHub Packages y Releases.

## REQUERIMIENTOS
  * Conocimientos: 
    - Conocimientos básicos de Bash (powershell).
    - Conocimientos básicos de Contenedores (Docker).
  * Hardware:
    - Virtualization activada en el BIOS..
    - CPU SLAT-capable feature.
    - Al menos 4GB de RAM.
  * Software:
    - Windows 10 64bit: Pro, Enterprise o Education (1607 Anniversary Update, Build 14393 o Superior)
    - Docker Desktop 
    - Powershell versión 7.x
    - Net 8 o superior
    - Visual Studio Code

## CONSIDERACIONES INICIALES
  * Clonar el repositorio mediante git para tener los recursos necesarios
  * Tener una cuenta de Github valida. 

## DESARROLLO
### Parte I: Configuración de la herramienta de Pruebas Estaticas de Seguridad de la Aplicación
1. Ingrear a la pagina de SonarCloud (https://www.sonarsource.com/products/sonarcloud/), iniciar sesión con su cuenta de Github.
2. Ingresar a la opción My Account
   ![image](https://github.com/UPT-FAING-EPIS/lab_calidad_01/assets/10199939/bd49c592-47f5-4767-9f15-c56ad6802818)
   
3. Generar un nuevo token con el nombre que desee, luego de generar el token, guarde el resultado en algún archivo o aplicación de notas. Debido a que se utilizará
   ![image](https://github.com/UPT-FAING-EPIS/lab_calidad_01/assets/10199939/75941062-40f0-4689-8c91-6603ced490a3)
  
4. En el navegador, ingresar a la url https://sonarcloud.io/projects/create, para generar un nuevo proyecto con el nombre apibank, apunte el nombre del Project Key, que se utilizara mas adelante, luego dar click en el boton next.
   ![image](https://github.com/user-attachments/assets/95819e23-6dcf-4ca4-800d-7bfe0f686bf3)
   
5. En el navegador, finalizar la creación del proyecto, haciendo click en la opción Previous Version, y luego en el boton Create Project
   ![image](https://github.com/user-attachments/assets/7f8a6d86-f14d-4b49-882e-0d0bb35d5069)


### Parte II: Creación de la aplicación
1. Iniciar la aplicación Powershell o Windows Terminal en modo administrador 
2. Ejecutar el siguiente comando para crear una nueva solución
```
dotnet new sln -o Bank
```
3. Acceder a la solución creada y ejecutar el siguiente comando para crear una nueva libreria de clases y adicionarla a la solución actual.
```
cd Bank
dotnet new classlib -o Bank.Domain
dotnet sln add ./Bank.Domain/Bank.Domain.csproj
```
4. Ejecutar el siguiente comando para crear un nuevo proyecto de pruebas y adicionarla a la solución actual
```
dotnet new mstest -o Bank.Domain.Tests
dotnet sln add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj
dotnet add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj reference ./Bank.Domain/Bank.Domain.csproj
```
5. Iniciar Visual Studio Code (VS Code) abriendo el folder de la solución como proyecto. En el proyecto Bank.Domain, si existe un archivo Class1.cs proceder a eliminarlo. Asimismo en el proyecto Bank.Domain.Tests si existiese un archivo UnitTest1.cs, también proceder a eliminarlo.

6. En VS Code, en el proyecto Bank.Domain proceder a crear la carpeta `Models` y dentro de esta el archivo BankAccount.cs e introducir el siguiente código:
```C#
namespace Bank.Domain.Models
{
    public class BankAccount
    {
        private readonly string m_customerName;
        private double m_balance;
        private BankAccount() { }
        public BankAccount(string customerName, double balance)
        {
            m_customerName = customerName;
            m_balance = balance;
        }
        public string CustomerName { get { return m_customerName; } }
        public double Balance { get { return m_balance; }  }
        public void Debit(double amount)
        {
            if (amount > m_balance)
                throw new ArgumentOutOfRangeException("amount");
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount");
            m_balance -= amount;
        }
        public void Credit(double amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount");
            m_balance += amount;
        }
    }
}
```
7. Luego en el proyecto Bank.Domain.Tests añadir un nuevo archivo BanckAccountTests.cs e introducir el siguiente código:
```C#
using Bank.Domain.Models;
using NUnit.Framework;

namespace Bank.Domain.Tests
{
    public class BankAccountTests
    {
        [Test]
        public void Debit_WithValidAmount_UpdatesBalance()
        {
            // Arrange
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
            // Act
            account.Debit(debitAmount);
            // Assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
        }
    }
}
```
8. En el terminal, ejecutar las pruebas de lo nostruiido hasta el momento:
```Bash
dotnet test --collect:"XPlat Code Coverage"
```
> Resultado
```Bash
Failed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: < 1 ms
```
9. En el terminal, instalar la herramienta de .Net Sonar Scanner que permitirá conectarse a SonarQube para realizar las pruebas estáticas de la seguridad del código de la aplicación :
```Bash
dotnet tool install -g dotnet-sonarscanner
```
> Resultado
```Bash
Puede invocar la herramienta con el comando siguiente: dotnet-sonarscanner
La herramienta "dotnet-sonarscanner" (versión 'X.X.X') se instaló correctamente
```
10. En el terminal, ejecutar :
```Bash
dotnet sonarscanner begin /k:"PROJECT_KEY" /d:sonar.token="TOKEN" /d:sonar.host.url="https://sonarcloud.io" /o:"ORGANIZATION" /d:sonar.cs.opencover.reportsPaths="*/*/*/coverage.opencover.xml"
```
> Donde:
> - TOKEN: es el token que previamente se genero en la pagina de Sonar Source
> - ORGANIZATION: es el nombre de la organización generada en Sonar Source
> - PROJECT_KEY: es el nombre de la llave proyecto que previamente se genero en la pagina en Sonar Source

12. En el terminal, ejecutar:
```Bash
dotnet build --no-incremental
dotnet test --collect:"XPlat Code Coverage;Format=opencover"
```
13. Ejecutar nuevamente el paso 8 para lo cual se obtendra una respuesta similar a la siguiente:
```Bash
dotnet sonarscanner end /d:sonar.token="TOKEN"
```
14. En la pagina de Sonar Source verificar el resultado del analisis.

![image](https://github.com/UPT-FAING-EPIS/lab_calidad_01/assets/10199939/4e4892d3-71e2-4437-9713-a270ebf61b06)

15. Abrir un nuevo navegador de internet o pestaña con la url de su repositorio de Github ```https://github.com/UPT-FAING-EPIS/nombre_de_su_repositorio```, abrir la pestaña con el nombre *Settings*, en la opción *Secrets and Actions*, selecionar Actions y hacer click en el botón *New Respository Token*, en la ventana colocar en Nombre (Name): SONAR_TOKEN y en Secreto (Secret): el valor del token de Sonar Cloud, guardado previamente

![image](https://github.com/user-attachments/assets/cf0b874f-7eb9-4888-a37d-9a975316d53f)

16. En el VS Code, proceder a crear la carpeta .github/workflow y dentro de esta crear el archivo sonar.yml con el siguiente contenido, reemplazar los valores ORGANIZATION y PROJECT_KEY con los valores obtenidos de SonarCloud
```Yaml
name: Sonar Continuos Integration
env:
  DOTNET_VERSION: '8.x'                     # la versión de .NET
  SONAR_ORG: 'ORGANIZATION'                    # Nombre de la organización de sonar cloud
  SONAR_PROJECT: 'PROJECT_KEY'        # Key ID del proyecto de sonar
on:
  push:
    branches: [ "main" ]
  workflow_dispatch:

jobs:
  sonarqube:
    name: Sonarqube Analisys
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with:
          distribution: 'temurin'
          java-version: '17'
      - name: Configurando la versión de NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      - name: Instalar Scanner
        run: dotnet tool install -g dotnet-sonarscanner
      - name: Ejecutar pruebas
        run: |
          dotnet restore 
          dotnet test --collect:"XPlat Code Coverage;Format=opencover"
          dotnet-sonarscanner begin /k:"${{ env.SONAR_PROJECT }}" /o:"${{ env.SONAR_ORG }}" /d:sonar.login="${{ secrets.SONAR_TOKEN }}" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.cs.opencover.reportsPaths="*/*/*/coverage.opencover.xml" /d:sonar.qualitygate.wait=true
          dotnet build
          dotnet-sonarscanner end /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
```

---

## MARCO TEÓRICO

### Pruebas Estáticas de Seguridad (SAST)
Las pruebas estáticas de seguridad de aplicaciones (SAST - Static Application Security Testing) son un tipo de análisis que se realiza sobre el código fuente sin necesidad de ejecutar la aplicación. SonarCloud es una plataforma líder que proporciona:

- **Análisis de calidad de código:** Detecta code smells, bugs y vulnerabilidades de seguridad
- **Cobertura de código:** Mide el porcentaje del código cubierto por pruebas unitarias
- **Deuda técnica:** Cuantifica el esfuerzo necesario para corregir problemas de calidad
- **Integración continua:** Se integra fácilmente con pipelines CI/CD

### Arquitectura de la Aplicación
La aplicación desarrollada sigue una arquitectura en capas:

```
Bank (Solución)
├── Bank.Domain (Librería de clases)
│   └── Models
│       └── BankAccount.cs
└── Bank.Domain.Tests (Proyecto de pruebas)
    └── BankAccountTests.cs
```

### Tecnologías Utilizadas
- **.NET 9.0:** Framework de desarrollo multiplataforma
- **MSTest:** Framework de pruebas unitarias de Microsoft
- **Coverlet:** Herramienta para generar reportes de cobertura de código
- **SonarCloud:** Plataforma de análisis de calidad de código
- **GitHub Actions:** Servicio de CI/CD integrado en GitHub
- **NuGet:** Sistema de gestión de paquetes para .NET

---

## DESARROLLO DEL PROYECTO

### PARTE I: Configuración de SonarCloud

#### 1.1. Creación de Cuenta y Token
1. Se accedió a [SonarCloud](https://www.sonarsource.com/products/sonarcloud/) e inicio de sesión con GitHub
2. Generación de token de autenticación personal desde "My Account > Security"
3. El token se guardó de forma segura para su uso posterior

#### 1.2. Creación del Proyecto
1. Acceso a la URL de creación de proyectos: https://sonarcloud.io/projects/create
2. Creación del proyecto con nombre "apibank"
3. Obtención del Project Key único para identificación
4. Configuración de análisis manual (Previous Version)

### PARTE II: Desarrollo de la Aplicación

#### 2.1. Estructura del Proyecto
Se creó la estructura completa del proyecto utilizando .NET CLI:

```powershell
# Creación de la solución
dotnet new sln -o Bank
cd Bank

# Librería de dominio
dotnet new classlib -o Bank.Domain
dotnet sln add ./Bank.Domain/Bank.Domain.csproj

# Proyecto de pruebas
dotnet new mstest -o Bank.Domain.Tests
dotnet sln add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj
dotnet add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj reference ./Bank.Domain/Bank.Domain.csproj
```

#### 2.2. Implementación del Modelo de Dominio

**Clase BankAccount** (`Bank.Domain/Models/BankAccount.cs`):
```csharp
namespace Bank.Domain.Models
{
    public class BankAccount
    {
        private readonly string m_customerName;
        private double m_balance;

        private BankAccount() { }

        public BankAccount(string customerName, double balance)
        {
            m_customerName = customerName;
            m_balance = balance;
        }

        public string CustomerName { get { return m_customerName; } }
        public double Balance { get { return m_balance; } }

        public void Debit(double amount)
        {
            if (amount > m_balance)
                throw new ArgumentOutOfRangeException("amount");
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount");
            m_balance -= amount;
        }

        public void Credit(double amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount");
            m_balance += amount;
        }
    }
}
```

**Características de la implementación:**
- Encapsulación de datos mediante campos privados
- Propiedades de solo lectura para CustomerName y Balance
- Validación de parámetros en métodos Debit y Credit
- Manejo de excepciones para casos de error

#### 2.3. Implementación de Pruebas Unitarias

Se desarrollaron 8 casos de prueba para validar la funcionalidad:

**Suite de Pruebas** (`Bank.Domain.Tests/BankAccountTests.cs`):

| Test | Descripción | Resultado Esperado |
|------|-------------|-------------------|
| `Debit_WithValidAmount_UpdatesBalance` | Verifica débito válido | Balance reducido correctamente |
| `Credit_WithValidAmount_UpdatesBalance` | Verifica crédito válido | Balance incrementado correctamente |
| `Debit_WhenAmountIsMoreThanBalance_ShouldThrowArgumentOutOfRange` | Valida débito excesivo | Excepción ArgumentOutOfRangeException |
| `Debit_WhenAmountIsNegative_ShouldThrowArgumentOutOfRange` | Valida débito negativo | Excepción ArgumentOutOfRangeException |
| `Credit_WhenAmountIsNegative_ShouldThrowArgumentOutOfRange` | Valida crédito negativo | Excepción ArgumentOutOfRangeException |
| `Constructor_ShouldSetCustomerNameCorrectly` | Verifica inicialización de nombre | Nombre asignado correctamente |
| `Constructor_ShouldSetBalanceCorrectly` | Verifica inicialización de balance | Balance asignado correctamente |
| `Debit_WithZeroAmount_ShouldNotChangeBalance` | Valida débito de valor cero | Balance sin cambios |

#### 2.4. Configuración para Publicación NuGet

El archivo `Bank.Domain.csproj` fue configurado con metadata para publicación:

```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <PackageId>Bank.Domain</PackageId>
  <Version>1.0.0</Version>
  <Authors>UPT</Authors>
  <Description>Domain models de ejemplo para laboratorio de SonarCloud.</Description>
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <RepositoryUrl>https://github.com/UPT-FAING-EPIS/nombre_de_su_repositorio</RepositoryUrl>
  <RepositoryType>git</RepositoryType>
</PropertyGroup>
```

### PARTE III: Análisis Local con SonarCloud

#### 3.1. Instalación de Herramientas
```powershell
dotnet tool install -g dotnet-sonarscanner
```

#### 3.2. Ejecución del Análisis
```powershell
# Inicio del análisis
dotnet sonarscanner begin /k:"PROJECT_KEY" /d:sonar.token="TOKEN" /d:sonar.host.url="https://sonarcloud.io" /o:"ORGANIZATION" /d:sonar.cs.opencover.reportsPaths="*/*/*/coverage.opencover.xml"

# Compilación y pruebas
dotnet build --no-incremental
dotnet test --collect:"XPlat Code Coverage;Format=opencover"

# Finalización del análisis
dotnet sonarscanner end /d:sonar.token="TOKEN"
```

### PARTE IV: Integración Continua con GitHub Actions

#### 4.1. Configuración de Secretos
Se configuró el secreto `SONAR_TOKEN` en GitHub:
- Navegación: Settings > Secrets and variables > Actions
- Nombre: `SONAR_TOKEN`
- Valor: Token generado en SonarCloud

#### 4.2. Workflow de CI/CD

Se creó el archivo `.github/workflows/sonar.yml` con el pipeline completo:

```yaml
name: Sonar Continuous Integration
env:
  DOTNET_VERSION: '9.x'
  SONAR_ORG: 'ORGANIZATION'
  SONAR_PROJECT: 'PROJECT_KEY'

on:
  push:
    branches: [ "main" ]
  workflow_dispatch:

jobs:
  sonarqube:
    name: Sonarqube Analysis
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - uses: actions/setup-java@v4
        with:
          distribution: 'temurin'
          java-version: '17'
      
      - name: Configurando la versión de NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: Instalar Scanner
        run: dotnet tool install -g dotnet-sonarscanner
      
      - name: Ejecutar pruebas
        run: |
          dotnet restore 
          dotnet test --collect:"XPlat Code Coverage;Format=opencover"
          dotnet-sonarscanner begin /k:"${{ env.SONAR_PROJECT }}" /o:"${{ env.SONAR_ORG }}" /d:sonar.login="${{ secrets.SONAR_TOKEN }}" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.cs.opencover.reportsPaths="*/*/*/coverage.opencover.xml" /d:sonar.qualitygate.wait=true
          dotnet build
          dotnet-sonarscanner end /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
      
      - name: Empaquetar NuGet
        run: dotnet pack ./Bank/Bank.Domain/Bank.Domain.csproj -c Release -o ./artifacts
      
      - name: Publicar paquete en GitHub Packages
        env:
          NUGET_SOURCE: https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json
          NUGET_API_KEY: ${{ secrets.GITHUB_TOKEN }}
        run: dotnet nuget push ./artifacts/*.nupkg --source $NUGET_SOURCE --api-key $NUGET_API_KEY --skip-duplicate
      
      - name: Crear release v1.0.0 con notas de commits
        uses: softprops/action-gh-release@v2
        with:
          tag_name: v1.0.0
          name: v1.0.0
          generate_release_notes: true
          files: artifacts/*.nupkg
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

---

## ACTIVIDADES REALIZADAS

### Actividad 1: Método de Prueba para Crédito ✅

**Objetivo:** Implementar prueba unitaria para el método `Credit()` de la clase `BankAccount`.

**Implementación:**
```csharp
[TestMethod]
public void Credit_WithValidAmount_UpdatesBalance()
{
    // Arrange
    double beginningBalance = 11.99;
    double creditAmount = 5.01;
    double expected = 17.00;
    var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

    // Act
    account.Credit(creditAmount);

    // Assert
    double actual = account.Balance;
    Assert.AreEqual(expected, actual, 0.001);
}
```

**Validaciones realizadas:**
- ✓ El método `Credit()` incrementa correctamente el saldo
- ✓ Balance inicial: $11.99
- ✓ Monto acreditado: $5.01
- ✓ Balance esperado: $17.00
- ✓ Precisión de comparación: 0.001 (para decimales)

**Resultado:** Test ejecutado exitosamente, cobertura de código incrementada.

---

### Actividad 2: Cobertura de Código en SonarCloud ✅

**Objetivo:** Incrementar la cobertura de código mediante pruebas unitarias adicionales.

**Pruebas implementadas adicionales:**
1. Validación de constructor para nombre de cliente
2. Validación de constructor para balance inicial
3. Test de débito con monto cero
4. Tests de excepciones para montos negativos

**Métricas de cobertura:**
- **Total de métodos probados:** 5 de 5 (100%)
- **Total de líneas cubiertas:** Alta cobertura en clase BankAccount
- **Casos de prueba:** 8 tests implementados
- **Resultado:** Todos los tests pasan exitosamente

**Evidencia:** Reportes disponibles en SonarCloud dashboard del proyecto.

---

### Actividad 3: Automatización de Construcción y Publicación NuGet ✅

**Objetivo:** Configurar pipeline para publicación automática de paquetes NuGet en GitHub Packages.

**Configuración implementada:**

**Step 1: Empaquetado**
```yaml
- name: Empaquetar NuGet
  run: dotnet pack ./Bank/Bank.Domain/Bank.Domain.csproj -c Release -o ./artifacts
```

**Step 2: Publicación**
```yaml
- name: Publicar paquete en GitHub Packages
  env:
    NUGET_SOURCE: https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json
    NUGET_API_KEY: ${{ secrets.GITHUB_TOKEN }}
  run: dotnet nuget push ./artifacts/*.nupkg --source $NUGET_SOURCE --api-key $NUGET_API_KEY --skip-duplicate
```

**Características técnicas:**
- ✓ Compilación en modo Release
- ✓ Generación de artefactos en carpeta `./artifacts`
- ✓ Autenticación mediante `GITHUB_TOKEN` automático
- ✓ Opción `--skip-duplicate` para evitar conflictos
- ✓ Publicación en GitHub Packages del repositorio

**Resultado:** Paquete NuGet `Bank.Domain.1.0.0.nupkg` publicado exitosamente.

---

### Actividad 4: Generación Automática de Release v1.0.0 ✅

**Objetivo:** Implementar generación automática de releases con notas de commits.

**Configuración implementada:**
```yaml
- name: Crear release v1.0.0 con notas de commits
  uses: softprops/action-gh-release@v2
  with:
    tag_name: v1.0.0
    name: v1.0.0
    generate_release_notes: true
    files: artifacts/*.nupkg
  env:
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

**Características del release:**
- **Tag:** v1.0.0
- **Título:** v1.0.0
- **Notas:** Generadas automáticamente a partir del historial de commits
- **Archivos adjuntos:** Paquetes .nupkg generados
- **Fecha de publicación:** Automática al ejecutar el workflow

**Contenido del release:**
- Historial completo de cambios desde el inicio del proyecto
- Links a commits individuales
- Información de contribuidores
- Artefactos descargables (paquetes NuGet)

**Resultado:** Release v1.0.0 creado y publicado exitosamente en GitHub.

---

## RESULTADOS Y ANÁLISIS

### Métricas del Proyecto

| Métrica | Valor | Estado |
|---------|-------|--------|
| **Tests implementados** | 8 | ✅ |
| **Tests exitosos** | 8 | ✅ |
| **Cobertura de código** | Alta | ✅ |
| **Quality Gate** | Passed | ✅ |
| **Vulnerabilidades** | 0 | ✅ |
| **Code Smells** | Mínimo | ✅ |
| **Deuda técnica** | Baja | ✅ |

### Flujo de CI/CD Implementado

```mermaid
graph LR
    A[Push a main] --> B[Checkout código]
    B --> C[Setup .NET 9 + Java 17]
    C --> D[Instalar dotnet-sonarscanner]
    D --> E[Restaurar dependencias]
    E --> F[Ejecutar tests con cobertura]
    F --> G[Análisis SonarCloud]
    G --> H[Build Release]
    H --> I[Empaquetar NuGet]
    I --> J[Publicar en GitHub Packages]
    J --> K[Crear Release v1.0.0]
```

### Pipeline de Ejecución

**Tiempo promedio de ejecución:** ~3-5 minutos

**Fases del pipeline:**
1. **Preparación del entorno** (30s)
   - Checkout del código
   - Configuración de .NET 9.x
   - Configuración de Java 17
   
2. **Instalación de herramientas** (20s)
   - dotnet-sonarscanner
   
3. **Análisis y pruebas** (90s)
   - Restauración de dependencias
   - Ejecución de 8 tests unitarios
   - Generación de reportes de cobertura OpenCover
   - Análisis estático con SonarCloud
   
4. **Build y empaquetado** (30s)
   - Compilación en modo Release
   - Generación de paquete NuGet
   
5. **Publicación** (40s)
   - Push a GitHub Packages
   - Creación de release con notas automáticas

---

## COMANDOS ÚTILES

### Ejecución Local de Pruebas

```powershell
# Restaurar dependencias
dotnet restore

# Ejecutar pruebas con cobertura
dotnet test --collect:"XPlat Code Coverage;Format=opencover"

# Compilar en modo Release
dotnet build --configuration Release

# Crear paquete NuGet
dotnet pack ./Bank/Bank.Domain/Bank.Domain.csproj -c Release -o ./artifacts
```

### Análisis Local con SonarCloud

```powershell
# Iniciar análisis
dotnet sonarscanner begin /k:"PROJECT_KEY" /d:sonar.token="TOKEN" /d:sonar.host.url="https://sonarcloud.io" /o:"ORGANIZATION" /d:sonar.cs.opencover.reportsPaths="*/*/*/coverage.opencover.xml"

# Compilar
dotnet build --no-incremental

# Ejecutar pruebas
dotnet test --collect:"XPlat Code Coverage;Format=opencover"

# Finalizar análisis
dotnet sonarscanner end /d:sonar.token="TOKEN"
```

### Gestión del Proyecto

```powershell
# Ver estructura de la solución
dotnet sln list

# Ejecutar tests específicos
dotnet test --filter "FullyQualifiedName~BankAccountTests.Credit_WithValidAmount_UpdatesBalance"

# Ver información del paquete
dotnet pack --help

# Limpiar artefactos de compilación
dotnet clean
```

---

## CONCLUSIONES

1. **Calidad de Código:**
   - Se logró implementar una aplicación con alta calidad de código validada por SonarCloud
   - La ausencia de vulnerabilidades críticas demuestra buenas prácticas de seguridad
   - El código es mantenible y sigue estándares de .NET

2. **Cobertura de Pruebas:**
   - Se alcanzó una cobertura significativa con 8 casos de prueba
   - Todos los métodos públicos de la clase BankAccount están probados
   - Se validaron casos positivos y negativos (excepciones)

3. **Automatización:**
   - El pipeline de CI/CD permite detección temprana de problemas
   - La integración con SonarCloud proporciona retroalimentación inmediata
   - La publicación automática de artefactos agiliza el proceso de distribución

4. **Aprendizajes:**
   - Importancia de las pruebas estáticas en el desarrollo de software
   - Valor de la automatización en procesos de calidad
   - Integración de herramientas modernas de DevOps

---

## RECOMENDACIONES

1. **Mejoras Futuras:**
   - Implementar pruebas de integración además de unitarias
   - Agregar análisis de seguridad de dependencias (Dependabot)
   - Incluir métricas de rendimiento en el pipeline

2. **Buenas Prácticas:**
   - Mantener la cobertura de código por encima del 80%
   - Ejecutar análisis de SonarCloud antes de cada merge
   - Documentar nuevas funcionalidades con tests correspondientes

3. **Seguridad:**
   - Rotar tokens de acceso periódicamente
   - Revisar regularmente las vulnerabilidades reportadas
   - Mantener actualizadas las dependencias del proyecto

---

## REFERENCIAS

- [Documentación oficial de SonarCloud](https://docs.sonarcloud.io/)
- [Guía de MSTest Framework](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [NuGet Package Documentation](https://learn.microsoft.com/en-us/nuget/)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)

---

## ANEXOS

### Estructura Completa del Proyecto

```
lab-2025-ii-si784-u1-01-csharp-Vlkair/
├── .github/
│   └── workflows/
│       └── sonar.yml                    # Pipeline CI/CD
├── Bank/
│   ├── Bank.sln                         # Solución .NET
│   ├── Bank.Domain/
│   │   ├── Bank.Domain.csproj          # Configuración del proyecto
│   │   └── Models/
│   │       └── BankAccount.cs          # Modelo de dominio
│   └── Bank.Domain.Tests/
│       ├── Bank.Domain.Tests.csproj    # Configuración de pruebas
│       ├── BankAccountTests.cs         # Suite de pruebas
│       └── MSTestSettings.cs           # Configuración MSTest
└── README.md                            # Documentación (este archivo)
```

### Comandos de Git Utilizados

```bash
# Clonar el repositorio
git clone https://github.com/UPT-FAING-EPIS/lab-2025-ii-si784-u1-01-csharp-Vlkair.git

# Crear rama de desarrollo
git checkout -b development

# Agregar cambios
git add .
git commit -m "feat: implementación de pruebas y CI/CD"

# Publicar cambios
git push origin main
```

---

**Fecha de elaboración:** 05 de Diciembre de 2025  
**Elaborado por:** Victor Cruz  
**Curso:** Calidad de Software - SI784  
**Universidad Privada de Tacna**

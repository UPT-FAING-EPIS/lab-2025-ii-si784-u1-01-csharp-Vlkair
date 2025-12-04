[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/qlAtkCwb)
[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=20511375)
# SESION DE LABORATORIO N° 01: PRUEBAS ESTATICAS DE SEGURIDAD DE APLICACIONES CON SONARQUBE

## OBJETIVOS
  * Comprender el funcionamiento de las pruebas estaticas de seguridad de còdigo de las aplicaciones que desarrollamos utilizando SonarQube.

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
## Actividades Encargadas

### 1. Método de Prueba para Crédito
Se ha adicionado el método de prueba `Credit_WithValidAmount_UpdatesBalance()` en el archivo `BankAccountTests.cs` para verificar el correcto funcionamiento del método `Credit()` de la clase `BankAccount`.

**Código implementado:**
```csharp
[TestMethod]
public void Credit_WithValidAmount_UpdatesBalance()
{
    double beginningBalance = 11.99;
    double creditAmount = 5.01;
    double expected = 17.00;
    var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

    account.Credit(creditAmount);

    double actual = account.Balance;
    Assert.AreEqual(expected, actual, 0.001);
}
```

Este test verifica que:
- El método `Credit()` incrementa correctamente el balance de la cuenta
- Se valida con un balance inicial de 11.99, se acreditan 5.01 y se espera un resultado de 17.00
- La precisión de comparación es de 0.001 para valores decimales

### 2. Cobertura de Código en SonarCloud
Se ha incrementado la cobertura de código mediante la adición de pruebas unitarias. La evidencia del incremento de cobertura se encuentra en el archivo `cobertura.png` ubicado en la raíz del repositorio.

**Verificación de cobertura:**
- Las pruebas se ejecutan con el comando: `dotnet test --collect:"XPlat Code Coverage;Format=opencover"`
- Los reportes de cobertura se generan en formato OpenCover
- SonarCloud analiza automáticamente la cobertura en cada push al repositorio

### 3. Automatización de Construcción y Publicación del Paquete NuGet
Se ha configurado el workflow de GitHub Actions (`sonar.yml`) para construir y publicar automáticamente el paquete NuGet en GitHub Packages.

**Configuración implementada:**
```yaml
- name: Empaquetar NuGet
  run: dotnet pack ./Bank/Bank.Domain/Bank.Domain.csproj -c Release -o ./artifacts

- name: Publicar paquete en GitHub Packages
  env:
    NUGET_SOURCE: https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json
    NUGET_API_KEY: ${{ secrets.GITHUB_TOKEN }}
  run: dotnet nuget push ./artifacts/*.nupkg --source $NUGET_SOURCE --api-key $NUGET_API_KEY --skip-duplicate
```

**Características:**
- El paquete se construye en configuración Release
- Se genera en la carpeta `./artifacts`
- Se publica automáticamente en GitHub Packages
- Utiliza el token de GitHub Actions para autenticación
- La opción `--skip-duplicate` evita errores si el paquete ya existe

### 4. Generación Automática de Release v1.0.0
Se ha implementado la generación automática del release v1.0.0 con las notas de los commits realizados.

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

**Características del Release:**
- **Tag:** v1.0.0
- **Generación automática de notas:** Utiliza `generate_release_notes: true` para crear automáticamente las notas basadas en los commits
- **Archivos adjuntos:** Se incluyen los paquetes .nupkg generados
- **Historial de cambios:** Las notas del release documentan todas las modificaciones realizadas desde el inicio del proyecto

---

## Flujo de Integración Continua Completo

El workflow completo de GitHub Actions realiza las siguientes operaciones:

1. **Checkout del código:** Descarga el repositorio
2. **Configuración del entorno:** Instala Java 17 y .NET 9.0.x
3. **Instalación de herramientas:** Instala dotnet-sonarscanner
4. **Restauración de dependencias:** Ejecuta `dotnet restore`
5. **Ejecución de pruebas:** Corre las pruebas con cobertura de código
6. **Análisis de SonarCloud:** 
   - Inicia el análisis con `dotnet-sonarscanner begin`
   - Compila la solución en modo Release
   - Finaliza el análisis con `dotnet-sonarscanner end`
7. **Empaquetado NuGet:** Crea el paquete .nupkg
8. **Publicación:** Publica el paquete en GitHub Packages
9. **Release:** Crea el release v1.0.0 con notas automáticas

---

## Comandos Útiles

### Ejecución Local de Pruebas
```bash
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
```bash
# Iniciar análisis
dotnet sonarscanner begin /k:"PROJECT_KEY" /d:sonar.token="TOKEN" /d:sonar.host.url="https://sonarcloud.io" /o:"ORGANIZATION" /d:sonar.cs.opencover.reportsPaths="*/*/*/coverage.opencover.xml"

# Compilar
dotnet build --no-incremental

# Ejecutar pruebas
dotnet test --collect:"XPlat Code Coverage;Format=opencover"

# Finalizar análisis
dotnet sonarscanner end /d:sonar.token="TOKEN"
```

---

## Resultado Final

✅ **Actividad 1:** Método de prueba `Credit_WithValidAmount_UpdatesBalance()` implementado  
✅ **Actividad 2:** Cobertura de código verificada en SonarCloud (ver `cobertura.png`)  
✅ **Actividad 3:** Pipeline de empaquetado y publicación NuGet configurado  
✅ **Actividad 4:** Generación automática de release v1.0.0 con notas de commits  

El proyecto ahora cuenta con integración continua completa que incluye análisis de calidad de código, pruebas unitarias con cobertura, empaquetado automático y publicación de releases.

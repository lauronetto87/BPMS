*** Instalar cli dotnet
dotnet tool install --global dotnet-ef 

*** gerar um ponto de migrations é o nome da migration gerada, deve ser alterada decorrente da implementação
dotnet ef migrations add [MIGRATION_NAME] --project SatelittiBpms.Data --startup-project SatelittiBpms

*** manda atualizar a database baseada na ultima migrations gerada
$env:ASPNETCORE_ENVIRONMENT = 'Local'
dotnet ef database update --project SatelittiBpms.Data --startup-project SatelittiBpms

*** caso queira voltar para uma migration especifica
dotnet ef database update <MIGRATION> --project SatelittiBpms.Data --startup-project SatelittiBpms

*** manda remover o arquivo migrations gerado
dotnet ef migrations remove --project SatelittiBpms.Data --startup-project SatelittiBpms


https://www.learnentityframeworkcore.com/migrations
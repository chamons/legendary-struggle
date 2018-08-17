Q=$(if $(V),,@)

build::
	$(Q) dotnet build Legendary-Struggle.sln --no-restore /nologo /v:m

test:: build
	$(Q) dotnet test -nologo /v:q test/LS.Core.Tests/LS.Core.Tests.csproj

test-fast:: build
	$(Q) dotnet test --no-build -nologo /v:q --filter `basename $(TEST_FILES) | sed 's/\.[^.]*$$//'` test/LS.Core.Tests/LS.Core.Tests.csproj

# 🕹️ Game Updater (GitHub Release Updater)

Este projeto é um atualizador de build em **Python** que baixa automaticamente a versão mais recente de Release em um repositório **privado** do GitHub. Ele é ideal para pipelines de distribuição de builds em ambientes controlados (testes, QA, publicadores, etc).

>[!NOTE]
>Esse projeto foi feito pensando em automatizar o processo de testes de builds locais.

> 🔐 Requer um token de acesso do GitHub com permissões de leitura em repositórios privados.

---

## 🚀 Funcionalidades

- Acessa o release mais recente do GitHub (via API)
- Faz download do asset da build (ex: `.zip`)
- Substitui a build atual da Unity por essa nova
- Empacotado como um executável `.exe` com PyInstaller
- Suporte a variáveis de ambiente com `.env`

---

## 📁 Estrutura do Projeto

```
game-updater/
│
├── main.py                     # Setup do Atualizador
├── requirements.txt            # Dependências do projeto
├── environmentGenerator.bat    # Automatiza a criação de ambiente virtual
├── environmentUpdater.bat      # Automatiza a instalação de dependências
├── executableGenerator.bat     # Script automático de build do .exe
└── dist/                       # Onde o .exe final é gerado
```

---

## 🛠️ Requisitos

- Python 3.8 ou superior
- GitHub token com permissão de leitura em repositórios privados
- Pip e venv instalados

---

## 🧪 Setup para Desenvolvimento

### 1. Clone o repositório

```bash
git clone https://github.com/guipsg/GitRelease-Updater.git
```

### 2. Crie e ative um ambiente virtual

```bash
cd GitRelease-Updater/game-updater
python -m venv venv
venv\Scripts\activate  # Windows
```
ou

Executar: `environmentGenerator.bat`



### 3. Instale as dependências

```bash
pip install -r requirements.txt
```
ou

Executar: `environmentUpdater.bat`


---

## ▶️ Executando o script

Edite o código Python: `main.py`
```python
# CONFIGURAÇÕES
GITHUB_TOKEN = "ghp_seu_token_aqui"
REPO_OWNER = "nome-do-dono"
REPO_NAME = "nome-do-repositorio"
ASSET_NAME = "nome-da-build.zip"  # nome do arquivo da build no release
BUILD_DIR = ".\build"  # pasta onde a build estará
TEMP_DIR = "temp_download"  # pasta temporária
```


Com o ambiente ativado:

```bash
python main.py
```
ou

Executar: `main_venv_runTest.bat`

---
### O script irá fazer o download do arquivo .zip que foi configurado e extrair em na pasta da configurada em `BUILD_DIR`

## 🏗️ Gerando o Executável `.exe`

>[!NOTE]
>Esse executável não é necessário gerar, mas pode ser bom caso queira ter um atualizador automatizado sem a necessidade de instalar esse repositório ou repetir todo o processo de setup em outro computador

Você pode compilar tudo em um `.exe` com PyInstaller. Use o script `executableGenerator.bat`:

```bash
executableGenerator.bat
```

Ou manualmente:

```bash
venv\Scripts\activate
pyinstaller --onefile --clean  main.py
```

O executável será gerado na pasta `dist/`.

---

## ⚠️ Observações

- O script assume que o nome do asset no release do GitHub é fixo (ex: `nome-da-build.zip`). Edite o script para comportar as necessidades do seu repositório.
- A build antiga da Unity será **substituída**. Faça backup se necessário.
- O caminho da build local é configurado na constante `BUILD_DIR` dentro do script.
- Pode ser que você precise dar a algum programa permissões de adm caso não consiga rodar corretamente, mas isso não é obrigatório.

---

## 🧼 Limpando a build

Para apagar os arquivos temporários gerados pelo PyInstaller:

```bash
rd /s /q build
rd /s /q dist
del *.spec
```
ou delete as pastas `build` `dist` e o arquivo `main.spec`


## 📄 Licença

Este projeto está licenciado sob os termos da MIT License.



## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou pull requests.

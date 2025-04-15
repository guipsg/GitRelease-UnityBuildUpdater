import os
import requests
from tqdm import tqdm
import zipfile
import shutil

# CONFIGURAÇÕES
GITHUB_TOKEN = "seu_token_aqui"
REPO_OWNER = "nome-do-dono"
REPO_NAME = "nome-do-repositorio"
ASSET_NAME = "nome-da-build.zip"  # nome do arquivo da build no release
BUILD_DIR = ".\build"  # pasta onde a build está
TEMP_DIR = "temp_download"  # pasta temporária

# HEADERS para autenticação
HEADERS = {
    "Authorization": f"token {GITHUB_TOKEN}",
    "Accept": "application/vnd.github.v3+json"
}

def get_latest_release():
    url = f"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases/latest"
    response = requests.get(url, headers=HEADERS)
    response.raise_for_status()
    return response.json()

def download_asset(asset_url, output_path):
    response = requests.get(asset_url, headers={**HEADERS, "Accept": "application/octet-stream"}, stream=True)
    response.raise_for_status()

    total = int(response.headers.get('content-length', 0))
    with open(output_path, 'wb') as file, tqdm(
        desc=output_path,
        total=total,
        unit='B',
        unit_scale=True,
        unit_divisor=1024,
    ) as bar:
        for data in response.iter_content(chunk_size=1024):
            file.write(data)
            bar.update(len(data))

def extract_and_replace(zip_path, target_dir):
    if os.path.exists(TEMP_DIR):
        shutil.rmtree(TEMP_DIR)
    os.makedirs(TEMP_DIR)

    with zipfile.ZipFile(zip_path, 'r') as zip_ref:
        zip_ref.extractall(TEMP_DIR)

    if os.path.exists(target_dir):
        shutil.rmtree(target_dir)
    shutil.copytree(TEMP_DIR, target_dir)
    shutil.rmtree(TEMP_DIR)

def main():
    print("🔍 Buscando último release...")
    release = get_latest_release()
    
    asset = next((a for a in release["assets"] if a["name"] == ASSET_NAME), None)
    if not asset:
        raise Exception(f"❌ Asset '{ASSET_NAME}' não encontrado no último release.")

    print("⬇️ Baixando asset...")
    download_asset(asset["url"], ASSET_NAME)

    print("📦 Extraindo e atualizando build...")
    extract_and_replace(ASSET_NAME, BUILD_DIR)

    print("✅ Build atualizada com sucesso!")

if __name__ == "__main__":
    main()

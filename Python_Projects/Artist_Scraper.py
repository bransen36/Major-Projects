import requests
from bs4 import BeautifulSoup
import csv

BASE_URL = "https://web.archive.org"
ARTIST_PAGES = [
    "/web/20121007172955/https://www.nga.gov/collection/anZ1.htm",
    "/web/20121007172955/https://www.nga.gov/collection/anZ2.htm",
    "/web/20121007172955/https://www.nga.gov/collection/anZ3.htm",
    "/web/20121007172955/https://www.nga.gov/collection/anZ4.htm"
]

artists = []

for page in ARTIST_PAGES:
    url = BASE_URL + page
    response = requests.get(url)
    soup = BeautifulSoup(response.content, 'html5lib')

    # Find the div containing the artists
    body_div = soup.find('div', class_='BodyText')

    if not body_div:
        print(f"Could not find artist list on {url}")
        continue

    # Remove navigation table if present
    alpha_nav = body_div.find('table', class_='AlphaNav')
    if alpha_nav:
        alpha_nav.decompose()

    # Extract artist names and URLs
    for row in body_div.find_all('tr'):
        cols = row.find_all('td')
        if len(cols) >= 2:
            link = cols[0].find('a')
            if link:
                artist_name = link.text.strip()
                artist_url = BASE_URL + link['href']
                artists.append([artist_name, artist_url])

# Write to CSV file
csv_filename = "z-artist-names.csv"
with open(csv_filename, mode='w', newline='', encoding='utf-8') as file:
    writer = csv.writer(file)
    writer.writerow(["Name", "Link"])
    writer.writerows(artists)

print(f"Scraped {len(artists)} artists. Data saved to {csv_filename}.")

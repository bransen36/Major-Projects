#Python program to scrape a website and save quotes.

import requests
from bs4 import BeautifulSoup
import csv

URL = "https://www.passiton.com"
BASE_URL = "https://www.passiton.com/inspirational-quotes"

page = requests.get(URL)
soup = BeautifulSoup(page.content, 'html5lib')

quotes = [] # store quotes in this list

table = soup.find('div', attrs= {'id': 'all_quotes'})

def fetch_quotes(url):
    """Fetches quotes from the given URL and returns a list of dictionaries."""
    response = requests.get(url)
    response.raise_for_status()  # Raise an error for bad responses (4xx, 5xx)

    soup = BeautifulSoup(response.content, 'html5lib')
    quotes_data = []

    # Locate the section containing quotes
    quotes_section = soup.find('div', attrs={'id': 'all_quotes'})

    # Extract each quote
    for row in quotes_section.find_all('div', attrs={'class': 'col-6 col-lg-3 text-center margin-30px-bottom sm-margin-30px-top'}):
        quote_text = row.img['alt']
        if '#' in quote_text:
            lines, author = quote_text.split('#', 1)
        else:
            lines, author = quote_text, "Unknown"

        quotes_data.append({
            'theme': row.h5.text.strip(),
            'url': f"{URL}{row.a['href']}",
            'image': row.img['src'],
            'lines': lines.strip(),
            'author': author.strip()
        })

    return quotes_data

def quotes_to_csv(quotes):
    filename = 'inspirational_quotes.csv'
    with open(filename, 'w', newline='') as f:
        w=csv.DictWriter(f, ['theme', 'url', 'image', 'lines', 'author'])
        w.writeheader()
        for quote in quotes:
            w.writerow(quote)
    print(f"Quotes saved to {filename}")

def display_quotes(quotes):
    """Displays quotes in a formatted manner."""
    if not quotes:
        print("No quotes available.")
        return

    print("\nInspirational Quotes:\n" + "=" * 50)
    for idx, quote in enumerate(quotes, start=1):
        print(f"\nQuote {idx}:")
        print(f"  Theme  : {quote['theme']}")
        print(f"  Quote  : \"{quote['lines']}\"")
        print(f"  Author : {quote['author']}")
        print(f"  URL    : {quote['url']}")
        print(f"  Image  : {quote['image']}")
        print("-" * 50)

if __name__ == "__main__":
    choice = input('Would you like to (1)output the scraped data, or (2)have it put in a .csv file?')
    if choice == '1':
        quotes_list = fetch_quotes(BASE_URL)
        display_quotes(quotes_list)
    elif choice == '2':
        quotes_list = fetch_quotes(BASE_URL)
        quotes_to_csv(quotes_list)

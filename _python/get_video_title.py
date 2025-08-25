

#pip install pytubefix

from pytubefix import YouTube
from pytubefix.cli import on_progress
import sys


def main():
    # sys.argv[0] is the script name, sys.argv[1] is the parameter
    url = sys.argv[1]
    #url = 'https://www.youtube.com/watch?v=lajEH2JQHEM&list=PLCSrmUCnKYncjVVGM2WaG7a-Bopy2ukbs&index=11'

    yt = YouTube(url)
    print(yt.title)

if __name__ == "__main__":
    sys.stdout.reconfigure(encoding='utf-8')
    main()
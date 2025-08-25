#pip install pytubefix

from pytubefix import YouTube
from pytubefix.cli import on_progress
import sys

def main():
    # sys.argv[0] is the script name, sys.argv[1] is the parameter
    url = sys.argv[1]
    output_path = sys.argv[2]

    #url = 'https://www.youtube.com/watch?v=kEB11PQ9Eo8'

    yt = YouTube(url)

    audio_stream = yt.streams.filter(only_audio=True).first()
    audio_stream.download(filename='audio_only.mp3', output_path=output_path)

if __name__ == "__main__":
    main()
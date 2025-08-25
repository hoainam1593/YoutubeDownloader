

#pip install pytubefix

from pytubefix import YouTube
from pytubefix.cli import on_progress
import sys

def main():
    # sys.argv[0] is the script name, sys.argv[1] is the parameter
    url = sys.argv[1]
    res = sys.argv[2]
    output_path = sys.argv[3]

    #url = 'https://www.youtube.com/watch?v=lajEH2JQHEM&list=PLCSrmUCnKYncjVVGM2WaG7a-Bopy2ukbs&index=11'
    #res = '720p'
    #output_path = 'D:/test'

    yt = YouTube(url)

    video_stream = yt.streams.filter(res=res, adaptive=True, only_video=True).first()
    video_stream.download(filename='video_only.mp4', output_path=output_path)

if __name__ == "__main__":
    main()
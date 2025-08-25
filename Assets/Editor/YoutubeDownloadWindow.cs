
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class YoutubeDownloadWindow : EditorWindow
{
    private string videoUrl = "";
    private string outputPath = "";
    private int chooseResIndex = 2;

    private readonly string[] videoRes = {
        "360p", "480p", "720p"
    };

    [MenuItem("-----YouTube Download----/Open")]
    private static void OnMenuClicked()
    {
        var window = (YoutubeDownloadWindow)GetWindow(typeof(YoutubeDownloadWindow));
        window.titleContent = new GUIContent("YouTube Download");
        window.Show();
    }

    private void OnGUI()
    {
        videoUrl = EditorGUILayout.TextField("YouTube URL", videoUrl);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);
        chooseResIndex = EditorGUILayout.Popup("Video Res", chooseResIndex, videoRes);

        if (GUILayout.Button("Download"))
        {
            var videoTitle = GeVideoTitle();
            videoTitle = videoTitle.Replace('|', '-');

            var randomPath = $"{Application.dataPath}/../{FileUtil.GetUniqueTempPathInProject()}";
            
            DownloadVideo(randomPath);
            DownloadAudio(randomPath);
            MergeVideoAndAudio(videoTitle, randomPath);

            EditorUtility.DisplayDialog("info", $"done: \"{videoTitle}.mp4\"", "OK");
        }
    }

    private string GeVideoTitle()
    {
        var scriptPath = $"{Application.dataPath}/../_python/get_video_title.py";
        
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = $"\"{scriptPath}\" \"{videoUrl}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.CreateNoWindow = true;
        start.StandardOutputEncoding = Encoding.UTF8;

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                return result.Trim();
            }
        }
    }

    private void DownloadVideo(string parentPath)
    {
        var scriptPath = $"{Application.dataPath}/../_python/download_video.py";
        
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = $"\"{scriptPath}\" \"{videoUrl}\" {videoRes[chooseResIndex]} \"{parentPath}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit(); 

            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError("Python error: " + error);

            if (!string.IsNullOrEmpty(output))
                UnityEngine.Debug.LogError(output);
        }
    }
    
    private void DownloadAudio(string parentPath)
    {
        var scriptPath = $"{Application.dataPath}/../_python/download_audio.py";
        
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = $"\"{scriptPath}\" \"{videoUrl}\" \"{parentPath}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                process.WaitForExit();
                string result = reader.ReadToEnd();
                Debug.LogError(result);
            }
        }
    }

    private void MergeVideoAndAudio(string videoTitle, string parentPath)
    {
        var inputVideo = $"{parentPath}/video_only.mp4";
        var inputAudio = $"{parentPath}/audio_only.mp3";
        var outputVideo = $"{outputPath}/{videoTitle}.mp4";
        
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = $"{Application.dataPath}/../_ffmpeg/ffmpeg.exe";
        start.Arguments = $"-i \"{inputVideo}\" -i \"{inputAudio}\" -c:v copy -c:a aac \"{outputVideo}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Debug.LogError(result);
            }
        }
    }
}

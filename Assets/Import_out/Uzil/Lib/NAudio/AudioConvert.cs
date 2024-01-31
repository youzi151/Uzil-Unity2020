using System;
using System.IO;
using System.Threading.Tasks;

using UnityEngine;

using Uzil;
using Uzil.BuiltinUtil;

using NAudio.Wave;

public enum AudioFormat {
	None, WAV, MP3
}

public static class AudioConvert {

	public static AudioClip FromData(byte[] data, AudioFormat format, DictSO args = null) {
		if (args == null) args = new DictSO();

		// TODO : args
		
		WAV wav = AudioConvert.GetWAV(data, format);

		AudioClip audioClip = AudioClip.Create("_anonymous", wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
		audioClip.SetData(wav.TotalChannel, 0);

		return audioClip;
	}

	public static async void FromDataAsync(byte[] data, AudioFormat format, Action<AudioClip> cb, DictSO args = null) {
		if (args == null) args = new DictSO();

		// TODO : args

		Task<WAV> task = Task.Run<WAV>(()=>{
			return AudioConvert.GetWAV(data, format);
		});

		bool isCancel = false;

		Uzil.Event evt = ApplicationEvent.onApplicationQuit;
		EventListener listener = evt.Add(()=>{
			isCancel = true;
		});

		WAV wav = await task;

		evt.RemoveListener(listener);

		if (isCancel) return;

		if (wav == null) {
			cb(null);
			return;
		}

		AudioClip audioClip = AudioClip.Create("_anonymous", wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
		audioClip.SetData(wav.TotalChannel, 0);

		cb(audioClip);
	}

	private static WAV GetWAV (byte[] data, AudioFormat format) {
		// Load the data into a stream
		MemoryStream stream = new MemoryStream(data);
		WaveStream waveStream;

		switch (format) {
			case AudioFormat.MP3:
				// Convert the data in the stream to WAV format
				Mp3FileReader mp3audio = new Mp3FileReader(stream);
				waveStream = WaveFormatConversionStream.CreatePcmStream(mp3audio);
				break;
			case AudioFormat.WAV:
				// Convert the data in the stream to WAV format
				WaveFileReader wavAudio = new WaveFileReader(stream);
				waveStream = WaveFormatConversionStream.CreatePcmStream(wavAudio);
				break;
			default:
				return null;
		}
	
		// Convert to WAV data
		return new WAV(AudioMemStream(waveStream).ToArray());
	}

	private static MemoryStream AudioMemStream(WaveStream waveStream){
		MemoryStream outputStream = new MemoryStream();
		using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat)) 
		{ 
			byte[] bytes = new byte[waveStream.Length]; 
			waveStream.Position = 0;
			waveStream.Read(bytes, 0, Convert.ToInt32(waveStream.Length)); 
			waveFileWriter.Write(bytes, 0, bytes.Length); 
			waveFileWriter.Flush(); 
		}
		return outputStream;
	}
}

/* From http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html */
public class WAV  {

	// convert two bytes to one float in the range -1 to 1
	static float bytesToFloat(byte firstByte, byte secondByte) {
		// convert two bytes to one short (little endian)
		short s = (short)((secondByte << 8) | firstByte);
		// convert to range from -1 to (just below) 1
		return s / 32768.0F;
	}

	static int bytesToInt(byte[] bytes,int offset=0){
		int value = 0;
		for (int i=0; i<4; i++){
			value |= ((int)bytes[offset+i]) << (i*8);
		}
		return value;
	}
	// properties
	public float[] TotalChannel{get; internal set;}
	public int ChannelCount {get;internal set;}
	public int SampleCount {get;internal set;}
	public int Frequency {get;internal set;}

	public WAV (byte[] wav) {

		// Determine if mono or stereo
		this.ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

		// Get the frequency
		this.Frequency = bytesToInt(wav, 24);

		// Get past all the other sub chunks to get to the data subchunk:
		int pos = 12;   // First Subchunk ID from 12 to 16

		// Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
		while (!(wav[pos]==100 && wav[pos+1]==97 && wav[pos+2]==116 && wav[pos+3]==97)) {
			pos += 4;
			int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
			pos += 4 + chunkSize;
		}
		pos += 8;

		// Pos is now positioned to start of actual sound data.
		this.SampleCount = (wav.Length - pos)/2;     // 2 bytes per sample (16 bit sound mono)
		if (this.ChannelCount == 2) this.SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

		// Allocate memory (right will be null if only mono sound)
		if (this.ChannelCount == 2) {
			this.TotalChannel = new float[this.SampleCount*2];
		} else {
			this.TotalChannel = new float[this.SampleCount];
		}

		// Write to double array/s:
		int totalIdx = 0;
		while (pos < wav.Length) {
			float left = bytesToFloat(wav[pos], wav[pos + 1]);
			this.TotalChannel[totalIdx] = left;

			totalIdx++;
			pos += 2;

			if (ChannelCount == 2) {
				float right = bytesToFloat(wav[pos], wav[pos + 1]);
				this.TotalChannel[totalIdx] = right;

				totalIdx++;
				pos += 2;
			}
		}
	}

	public override string ToString (){
		return string.Format ("[WAV: TotalChannel={0}, ChannelCount={1}, SampleCount={2}, Frequency={3}]", TotalChannel, ChannelCount, SampleCount, Frequency);
	}
}
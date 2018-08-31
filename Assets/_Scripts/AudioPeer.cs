using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(AudioSource))]
public class AudioPeer : MonoBehaviour {
	AudioSource _audioSource;
	public static float[] _freqBand = new float[8]; 
	public static float[] _samples = new float[512];
	public static float[] _bandBuffer = new float[8];
	float[] _bufferDecrease = new float[8];
	void Start () 
	{
		_audioSource = GetComponent<AudioSource> ();		
	}

	void Update ()
	{
		GetSpectrumAudioSource ();
		MakeFrequencyBands ();
		BandBuffer ();
	}
	void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
	}

	void BandBuffer()
	{
		for(int g=0;g<8;g++)
		{
			if (_freqBand[g] > _bandBuffer[g]) {
				_bandBuffer [g] = _freqBand [g];
				_bufferDecrease [g] = 0.005f;
			}
			if (_freqBand[g] < _bandBuffer[g]) {
				_bandBuffer[g] -= _bufferDecrease [g];
				_bufferDecrease [g] *= 1.2f;
			}
		}
	}

	void MakeFrequencyBands()
	{
		/*
		 *  22050 / 512 = 43 hz per sample 
		 * 20-60hz
		 * 60-250hz
		 * 250-500hz
		 * 500-2000hz
		 * 2000-4000hz
		 * 4000-6000hz
		 * 6000-20000hz
		 * 
		 * 0-2   = 86hz
		 * 1-4   = 172hz   - 87-258
		 * 2-8   = 344hz   - 259-602 
		 * 3-16	 = 688hz   - 603-1290 
		 * 4-32  = 1376hz  - 1291-2666
		 * 5-64  = 2752hz  - 2667-5418
		 * 6-128 = 5504hz  - 5419-10922
		 * 7-256 = 11008hz - 10923-21930
		 * 510
		 */
		int count = 0;
		float average = 0;
		for (int i = 0; i < 8; i++) {
			int sampleCount = (int)Mathf.Pow (2, i) * 2;

			if (i == 7) {
				sampleCount += 2;
			} 
			for (int j = 0; j < sampleCount; j++) {
				average += _samples [count] * (count + 1);
				count++;
			}

			average /= count;
			_freqBand [i] = average*10;
		}
	}
}

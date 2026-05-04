window.waveformPlayer = {
    instances: {},

    init(elementId, audioUrl) {
        const container = document.getElementById(elementId);
        if (!container) return;

        this.destroy(elementId);

        const instance = {
            audio: new Audio(),
            audioContext: null,
            analyser: null,
            source: null,
            canvas: container.querySelector('.waveform-canvas'),
            ctx: null,
            isPlaying: false,
            animationId: null,
            duration: 0,
            waveformLoaded: false
        };

        instance.ctx = instance.canvas.getContext('2d');
        instance.audio.crossOrigin = 'anonymous';
        instance.audio.src = audioUrl;

        const playBtn = container.querySelector('.waveform-play-btn');
        const progressContainer = container.querySelector('.waveform-progress');
        const visualContainer = container.querySelector('.waveform-visual');

        playBtn.addEventListener('click', () => this.togglePlay(elementId));
        progressContainer.addEventListener('click', (e) => this.seek(elementId, e));
        visualContainer.addEventListener('click', (e) => this.seek(elementId, e));

        instance.audio.addEventListener('loadedmetadata', () => {
            instance.duration = instance.audio.duration;
        });

        instance.audio.addEventListener('timeupdate', () => {
            const percent = (instance.audio.currentTime / instance.duration) * 100;
            progressContainer.querySelector('.waveform-progress-fill').style.width = percent + '%';
            container.querySelector('.waveform-transport').style.left = percent + '%';
        });

        instance.audio.addEventListener('ended', () => {
            instance.isPlaying = false;
            playBtn.classList.remove('playing');
            cancelAnimationFrame(instance.animationId);
        });

        instance.audio.addEventListener('error', () => {
            this.drawError(instance.ctx, instance.canvas);
        });

        this.instances[elementId] = instance;
        this.loadWaveform(elementId, audioUrl);
    },

    async loadWaveform(elementId, audioUrl) {
        const instance = this.instances[elementId];
        if (!instance) return;

        try {
            const response = await fetch(audioUrl);
            const arrayBuffer = await response.arrayBuffer();
            
            instance.audioContext = new (window.AudioContext || window.webkitAudioContext)();
            const audioBuffer = await instance.audioContext.decodeAudioData(arrayBuffer);
            
            const rawData = audioBuffer.getChannelData(0);
            const samples = 150;
            const blockSize = Math.floor(rawData.length / samples);
            const peaks = [];

            for (let i = 0; i < samples; i++) {
                let max = 0;
                const start = i * blockSize;
                for (let j = 0; j < blockSize; j++) {
                    const val = Math.abs(rawData[start + j] || 0);
                    if (val > max) max = val;
                }
                peaks.push(max);
            }

            instance.peaks = peaks;
            instance.waveformLoaded = true;
            this.drawWaveform(elementId, 0);
        } catch (e) {
            console.error('Failed to load waveform:', e);
            this.drawFallbackWaveform(elementId);
        }
    },

    drawFallbackWaveform(elementId) {
        const instance = this.instances[elementId];
        if (!instance) return;

        const peaks = [];
        for (let i = 0; i < 150; i++) {
            peaks.push(Math.random() * 0.8 + 0.1);
        }
        instance.peaks = peaks;
        instance.waveformLoaded = true;
        this.drawWaveform(elementId, 0);
    },

    drawWaveform(elementId, progress) {
        const instance = this.instances[elementId];
        if (!instance || !instance.peaks) return;

        const { ctx, canvas, peaks } = instance;
        const width = canvas.width;
        const height = canvas.height;
        const centerY = height / 2;

        ctx.clearRect(0, 0, width, height);

        const barWidth = (width / peaks.length) * 0.6;
        const gap = (width / peaks.length) * 0.4;
        const progressX = width * (progress / 100);

        peaks.forEach((peak, i) => {
            const x = i * (barWidth + gap) + gap / 2;
            const barHeight = peak * (height * 0.8);
            const y = centerY - barHeight / 2;

            if (x < progressX) {
                ctx.fillStyle = '#5D5FEF';
                ctx.shadowColor = '#5D5FEF';
                ctx.shadowBlur = 4;
            } else {
                ctx.fillStyle = 'rgba(93, 95, 239, 0.3)';
                ctx.shadowColor = 'transparent';
                ctx.shadowBlur = 0;
            }

            ctx.beginPath();
            ctx.roundRect(x, y, barWidth, barHeight, 2);
            ctx.fill();
        });

        ctx.shadowBlur = 0;
    },

    drawError(ctx, canvas) {
        const width = canvas.width;
        const height = canvas.height;
        ctx.clearRect(0, 0, width, height);
        ctx.fillStyle = 'rgba(239, 83, 80, 0.5)';
        ctx.font = '14px sans-serif';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText('Failed to load audio', width / 2, height / 2);
    },

    togglePlay(elementId) {
        const instance = this.instances[elementId];
        if (!instance) return;

        const playBtn = document.querySelector(`#${elementId} .waveform-play-btn`);

        if (instance.isPlaying) {
            instance.audio.pause();
            instance.isPlaying = false;
            playBtn.classList.remove('playing');
            cancelAnimationFrame(instance.animationId);
        } else {
            instance.audio.play();
            instance.isPlaying = true;
            playBtn.classList.add('playing');
            this.animate(elementId);
        }
    },

    animate(elementId) {
        const instance = this.instances[elementId];
        if (!instance || !instance.isPlaying) return;

        const progress = (instance.audio.currentTime / instance.duration) * 100;
        const container = document.getElementById(elementId);
        if (container) {
            container.querySelector('.waveform-transport').style.left = progress + '%';
            container.querySelector('.waveform-progress-fill').style.width = progress + '%';
        }
        this.drawWaveform(elementId, progress);
        instance.animationId = requestAnimationFrame(() => this.animate(elementId));
    },

    seek(elementId, event) {
        const instance = this.instances[elementId];
        if (!instance || !instance.duration) return;

        const rect = event.currentTarget.getBoundingClientRect();
        const percent = (event.clientX - rect.left) / rect.width;
        instance.audio.currentTime = percent * instance.duration;
    },

    formatTime(seconds) {
        if (isNaN(seconds)) return '0:00';
        const mins = Math.floor(seconds / 60);
        const secs = Math.floor(seconds % 60);
        return `${mins}:${secs.toString().padStart(2, '0')}`;
    },

    destroy(elementId) {
        const instance = this.instances[elementId];
        if (!instance) return;

        if (instance.audio) {
            instance.audio.pause();
            instance.audio.src = '';
        }
        if (instance.animationId) {
            cancelAnimationFrame(instance.animationId);
        }
        if (instance.audioContext) {
            instance.audioContext.close();
        }
        delete this.instances[elementId];
    }
};

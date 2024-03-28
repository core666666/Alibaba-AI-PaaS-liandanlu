new Vue({
    el: '#app',
    data: {
        newQuestion: '',
        messages: [],
        selectedModel: 'chatMemo', //
        sending: false, // 添加一个标志，以禁用Send按钮
        recordershowModal: false,
        recorder: null,
        translatedText: '',
        isRecording: false,
        recordingDuration: 0, // in seconds
        hasRecording: false,
    },
    methods: {
        sendQuestion() {
            if (!this.newQuestion || this.sending) return;

            this.sending = true;

            let prompt = this.newQuestion;

            this.messages.push({ text: this.newQuestion, type: 'question' });

            fetch('https://test.cn/api/v1.0/aiPaaS/ai/gmemo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    modelId: this.selectedModel,
                    prompt: prompt,
                    chatType: this.selectedModel === "chatMemo" ? 1 : 0,
                })
            })
                .then(response => {
                    if (response.status === 500) throw new Error('模型不可用，请切换其他模型');
                    return response.body.getReader();
                })
                .then(reader => {
                    return new ReadableStream({
                        start(controller) {
                            function push() {
                                reader.read().then(({ done, value }) => {
                                    if (done) {
                                        controller.close();
                                        return;
                                    }
                                    controller.enqueue(value);
                                    push();
                                });
                            }
                            push();
                        }
                    });
                })
                .then(stream => {
                    const reader = stream.getReader();
                    let buffer = '';
                    const self = this;

                    return new Promise((resolve, reject) => {
                        function processText({ done, value }) {
                            if (done) {
                                resolve();
                                return;
                            }

                            buffer += new TextDecoder("utf-8").decode(value);
                            const lines = buffer.split('\n');

                            for (let i = 0; i < lines.length - 1; i++) {
                                const line = lines[i].trim();
                                if (line.startsWith('{') && line.endsWith('}')) {
                                    const data = JSON.parse(line);
                                    if (data.isFinished) {
                                        self.messages[self.messages.length - 1].text += data.data;
                                        self.$nextTick(() => {
                                            self.scrollToBottom();
                                        });
                                    } else {
                                        if (!self.messages.length || self.messages[self.messages.length - 1].type !== 'answer') {
                                            self.messages.push({ text: data.data, type: 'answer' });
                                        } else {
                                            self.messages[self.messages.length - 1].text += data.data;
                                        }
                                        self.$nextTick(() => {
                                            self.scrollToBottom();
                                        });
                                    }
                                }
                            }

                            buffer = lines[lines.length - 1];

                            return reader.read().then(processText);
                        }
                        reader.read().then(processText);
                    });
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert(error.message);
                })
                .finally(() => {
                    this.sending = false;
                    this.newQuestion = '';
                    this.$nextTick(() => {
                        this.scrollToBottom();
                    });
                });

            this.newQuestion = '';
            this.$nextTick(() => {
                this.scrollToBottom();
            });
        },
        clearChatScreen() {
            this.isCleared = true;
        },
        scrollToBottom() {
            const chatContainer = this.$el.querySelector(".chat-container");
            chatContainer.scrollTop = chatContainer.scrollHeight;
        },
        closeModal() {
            this.$nextTick(() => {
                this.$refs.questionInput.focus();
            });
        },
        disableContext() {
            this.contextLength = 1;
            this.closeModal();
            this.$nextTick(() => {
                this.$refs.questionInput.focus();
            });
        },
        startRecord() {
            if (!this.recorder) {
                this.recorder = new Recorder({
                    sampleRate: 16000
                });
            }
            this.recordershowModal = true;
            this.recorder.start();
            this.isRecording = true;
            this.hasRecording = false;
            this.recordingDuration = 0;
            console.log('录音中...');
            this.startTime = Date.now();
        },
        endRecord() {
            if (this.recorder) {
                this.recorder.stop();
                this.isRecording = false;
                this.hasRecording = true;
                this.recordingDuration = Math.round((Date.now() - this.startTime) / 1000);
                console.log(`录音结束，本次录音时间${this.recordingDuration}秒`);
                this.recordershowModal = false;
                this.transRecord();
            }
        },
        playRecord() {
            if (this.hasRecording && this.recorder) {
                this.recorder.play();
            } else {
                console.log('没有录音可以播放');
            }
        },
        transRecord() {
            if (this.recorder) {
                let pcm = this.recorder.getPCMBlob();
                let formdata = new FormData();
                formdata.append('file', pcm);

                fetch('https://test.cn/api/v1.0/aiPaaS/ai/speech', {
                    method: 'POST',
                    credentials: 'include',
                    body: formdata
                })
                    .then(response => response.json())
                    .then(data => {
                        let translatedTextStr = data.translatedText;
                        let translatedTextObj;

                        try {
                            translatedTextObj = JSON.parse(translatedTextStr);
                        } catch (error) {
                            console.error('Parsing error:', error);
                        }

                        if (translatedTextObj) {
                            if (translatedTextObj.status === 20000000) {
                                this.newQuestion = translatedTextObj.result;
                            } else {
                                console.log('Error with status:', translatedTextObj.status);
                                alert("未知错误，请联系管理员~");
                            }
                        } else {
                            alert("解析错误，请检查数据格式！");
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                    });
            }
        },
        toggleRecording() {
            if (!this.isRecording) {
                this.startRecord();
            } else {
                this.endRecord();
            }
        },
        navigateTo(url) {
            window.location.href = url;
        }
    },
    created() {
    }
});

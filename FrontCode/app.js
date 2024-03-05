new Vue({
    el: '#app',
    data: {
        newQuestion: '',
        messages: [],
        selectedModel: 'model-id', // 默认选中 Qwen-7B-Chat 模型
        contextLength: 5, // 设置默认上下文长度为5
        sending: false, // 添加一个标志，以禁用Send按钮
        showModal: true,
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

            let historicalQuestions = this.messages.filter(message => message.type === 'question');
            if (this.contextLength > 0 && historicalQuestions.length > this.contextLength) {
                historicalQuestions = historicalQuestions.slice(-this.contextLength);
            }

            let prompt = "" +
                historicalQuestions.map(message => message.text).join('；') +
                "，" + this.newQuestion;

            // Add the question to the messages array
            this.messages.push({ text: this.newQuestion, type: 'question' });

            // Send the question to the API
            fetch('https://test.cn/api/v1.0/aiPaaS/ai/g', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    module: this.selectedModel === 'model-id1' ? "Qwen-7B-Chat" :
                    this.selectedModel === 'model-id2' ? "MPT-7B" : 
                    "公积金专员",
                    modelId: this.selectedModel,//"model-qwen7b111chat666-0da7",
                    prompt: prompt,
                    userId: "1"
                })
            })
                .then(response => {
                    if (response.status === 500) {
                        this.sending = false;
                        throw new Error('模型不可用，请切换其他模型'); 
                    }
                    return response.json(); // 继续处理正常情况
                })
                .then(data => {
                    //this.messages.push({ text: this.newQuestion, type: 'question' });
                    this.simulateStreamingResponse(data.result.content);
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert(error.message);
                })
                .finally(() => {
                    //this.sending = false; 
                    this.newQuestion = ''; 
                    this.$nextTick(() => {
                        this.scrollToBottom(); // 滚动到底部
                    });
                });

            // Clear the question input field
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
        simulateStreamingResponse(content) {
            let index = 1;
            // 在messages数组中添加一个新的答案对象，并保存其位置
            const answerIndex = this.messages.push({ text: '', type: 'answer' }) - 1;

            const interval = setInterval(() => {
                if (index < content.length) {
                    // 逐步揭示内容直到整个答案被展示出来
                    const text = content.slice(0, index++);
                    this.messages[answerIndex].text = text;
                } else {
                    clearInterval(interval);
                    this.sending = false;
                    this.$nextTick(this.scrollToBottom); // 确保滚动到最新的消息
                }
            }, 100); // 这个时间间隔控制了打字效果的快慢
        },
        closeModal() {
            this.showModal = false; 
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
            //alert('录音中...');
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
                //alert(`录音结束，本次录音时间${this.recordingDuration}秒`);
            }
        },
        playRecord() {
            if (this.hasRecording && this.recorder) {
                this.recorder.play();
            } else {
                console.log('没有录音可以播放');
                //alert('没有录音可以播放');
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
                            //alert(translatedTextObj.result); 
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
        }
    },
    created() {
        // 页面加载完成时，可以决定是否立即显示模态框
        this.showModal = true;
    }
});

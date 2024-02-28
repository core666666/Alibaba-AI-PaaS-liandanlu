new Vue({
    el: '#app',
    data: {
        newQuestion: '',
        messages: [],
        selectedModel: 'model-qwen', // 默认选中 Qwen-7B-Chat 模型
        contextLength: 5, // 设置默认上下文长度为5
        sending: false, // 添加一个标志，以禁用Send按钮
        showModal: true,
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
            fetch('http://xxx/api/v1.0/aiPaaS/ai/g', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    module: this.selectedModel === 'model-qwen' ? "Qwen-7B-Chat" : "MPT-7B",
                    modelId: this.selectedModel,//"model-qwen",
                    prompt: prompt,
                    userId: "1"
                })
            })
                .then(response => {
                    if (response.status === 500) {
                        this.sending = false;
                        throw new Error('模型不可用，请切换其他模型'); // 抛出错误，将被.catch()捕获
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
                    //this.sending = false; // 请求结束，无论成功还是失败都要重置sending状态
                    this.newQuestion = ''; // 清空输入框
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
            this.showModal = false; // 关闭模态框
            this.$nextTick(() => {
                this.$refs.questionInput.focus(); // 调用 focus 方法
            });
        },
        disableContext() {
            this.contextLength = 1; // 将上下文长度设置为1
            this.closeModal(); // 关闭模态框
            this.$nextTick(() => {
                this.$refs.questionInput.focus(); // 调用 focus 方法
            });
        }
    },
    created() {
        // 页面加载完成时，可以决定是否立即显示模态框
        this.showModal = true;
    }
});

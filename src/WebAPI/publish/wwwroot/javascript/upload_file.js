window.readFile = async () => {
    return new Promise((resolve, reject) => {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = '.txt,text/plain'; // 只允许文本文件

        input.onchange = async () => {
            const file = input.files[0];
            if (!file) {
                resolve(null);
                return;
            }

            const reader = new FileReader();
            reader.onload = () => resolve(reader.result); // 返回文件内容字符串
            reader.onerror = () => reject('读取文件失败');
            reader.readAsText(file);
        };

        input.click(); // 触发文件选择
    });
};
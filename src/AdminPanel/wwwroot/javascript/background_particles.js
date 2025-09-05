window.initializeParticleBackground = () => {
    const canvas = document.getElementById('particleCanvas');
    const ctx = canvas.getContext('2d');

    // 设置画布大小
    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }

    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);

    // 粒子类
    class Particle {
        constructor() {
            this.x = Math.random() * canvas.width;
            this.y = Math.random() * canvas.height;
            this.size = Math.random() * 3 + 1;
            this.speedX = Math.random() * 1 - 0.5;
            this.speedY = Math.random() * 1 - 0.5;
            this.color = `rgba(255, 255, 255, ${Math.random() * 0.5 + 0.1})`;
        }

        update() {
            this.x += this.speedX;
            this.y += this.speedY;

            // 边界检查
            if (this.x > canvas.width || this.x < 0) this.speedX = -this.speedX;
            if (this.y > canvas.height || this.y < 0) this.speedY = -this.speedY;
        }

        draw() {
            ctx.fillStyle = this.color;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fill();
        }
    }

    // 创建粒子数组
    const particles = [];
    const particleCount = Math.min(100, Math.floor((window.innerWidth * window.innerHeight) / 5000));

    for (let i = 0; i < particleCount; i++) {
        particles.push(new Particle());
    }

    // 连线距离
    const connectDistance = 100;

    // 动画循环
    function animate() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // 更新和绘制粒子
        for (let i = 0; i < particles.length; i++) {
            particles[i].update();
            particles[i].draw();
        }

        // 绘制粒子之间的连线
        for (let i = 0; i < particles.length; i++) {
            for (let j = i + 1; j < particles.length; j++) {
                const dx = particles[i].x - particles[j].x;
                const dy = particles[i].y - particles[j].y;
                const distance = Math.sqrt(dx * dx + dy * dy);

                if (distance < connectDistance) {
                    const opacity = 1 - distance / connectDistance;
                    ctx.strokeStyle = `rgba(255, 255, 255, ${opacity * 0.2})`;
                    ctx.lineWidth = 0.5;
                    ctx.beginPath();
                    ctx.moveTo(particles[i].x, particles[i].y);
                    ctx.lineTo(particles[j].x, particles[j].y);
                    ctx.stroke();
                }
            }
        }

        requestAnimationFrame(animate);
    }

    animate();

    // 鼠标交互
    let mouseX = undefined;
    let mouseY = undefined;

    canvas.addEventListener('mousemove', (e) => {
        mouseX = e.x;
        mouseY = e.y;
    });

    canvas.addEventListener('mouseleave', () => {
        mouseX = undefined;
        mouseY = undefined;
    });
};


// LostBoy Web — Canvas renderer and keyboard handler

window.LostBoyGame = {
    canvas: null,
    ctx: null,
    cellW: 12,
    cellH: 18,
    keys: {},
    keysJustPressed: {},
    dotNetRef: null,

    init: function (canvasId, dotNetRef) {
        this.canvas = document.getElementById(canvasId);
        this.ctx = this.canvas.getContext('2d');
        this.dotNetRef = dotNetRef;
        this.keys = {};
        this.keysJustPressed = {};

        // Keyboard listeners on window so we always capture
        window.addEventListener('keydown', (e) => {
            e.preventDefault();
            const key = e.key.toLowerCase();
            if (!this.keys[key]) {
                this.keysJustPressed[key] = true;
            }
            this.keys[key] = true;
        });

        window.addEventListener('keyup', (e) => {
            e.preventDefault();
            const key = e.key.toLowerCase();
            this.keys[key] = false;
            this.keysJustPressed[key] = false;
        });

        return true;
    },

    setSize: function (cols, rows) {
        this.canvas.width = cols * this.cellW;
        this.canvas.height = rows * this.cellH;
    },

    clear: function () {
        this.ctx.fillStyle = '#0a0a0f';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
    },

    // Draw a single character at grid position
    drawCell: function (x, y, ch, color) {
        const px = x * this.cellW;
        const py = y * this.cellH;

        // Clear cell
        this.ctx.fillStyle = '#0a0a0f';
        this.ctx.fillRect(px, py, this.cellW, this.cellH);

        // Draw character
        this.ctx.fillStyle = color;
        this.ctx.font = '14px "JetBrains Mono", "Fira Code", "Cascadia Code", monospace';
        this.ctx.textBaseline = 'top';
        this.ctx.fillText(ch, px + 1, py + 1);
    },

    // Batch draw — receives flat arrays for performance
    drawBatch: function (xs, ys, chars, colors) {
        const ctx = this.ctx;
        ctx.font = '14px "JetBrains Mono", "Fira Code", "Cascadia Code", monospace';
        ctx.textBaseline = 'top';

        for (let i = 0; i < xs.length; i++) {
            const px = xs[i] * this.cellW;
            const py = ys[i] * this.cellH;

            ctx.fillStyle = '#0a0a0f';
            ctx.fillRect(px, py, this.cellW, this.cellH);

            ctx.fillStyle = colors[i];
            ctx.fillText(chars[i], px + 1, py + 1);
        }
    },

    // Draw a string starting at grid position
    drawString: function (x, y, text, color) {
        const ctx = this.ctx;
        ctx.font = '14px "JetBrains Mono", "Fira Code", "Cascadia Code", monospace';
        ctx.textBaseline = 'top';

        for (let i = 0; i < text.length; i++) {
            const px = (x + i) * this.cellW;
            const py = y * this.cellH;

            ctx.fillStyle = '#0a0a0f';
            ctx.fillRect(px, py, this.cellW, this.cellH);

            ctx.fillStyle = color;
            ctx.fillText(text[i], px + 1, py + 1);
        }
    },

    // Draw a filled rectangle (for boxes, bars)
    drawRect: function (x, y, w, h, color) {
        this.ctx.fillStyle = color;
        this.ctx.fillRect(x * this.cellW, y * this.cellH, w * this.cellW, h * this.cellH);
    },

    // Check if a key is currently held down
    isKeyDown: function (key) {
        return !!this.keys[key.toLowerCase()];
    },

    // Check if a key was just pressed (one-shot)
    consumeKey: function (key) {
        const k = key.toLowerCase();
        if (this.keysJustPressed[k]) {
            this.keysJustPressed[k] = false;
            return true;
        }
        return false;
    },

    // Clear all just-pressed flags (call at end of each frame)
    clearFrameInput: function () {
        // Don't clear — consumeKey handles one-shot reads
    }
};

gsap.from(".animate-page", {
    opacity: 0,
    y: 30,
    duration: 0.8,
    ease: "power3.out"
});

document.querySelectorAll("button").forEach(btn => {
    btn.addEventListener("mouseenter", () =>
        gsap.to(btn, { scale: 1.05, duration: 0.15 })
    );
    btn.addEventListener("mouseleave", () =>
        gsap.to(btn, { scale: 1.0, duration: 0.15 })
    );
});

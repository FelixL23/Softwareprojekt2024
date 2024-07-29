// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const navLinks = document.querySelectorAll("nav a");

  navLinks.forEach((link) => {
    link.classList.remove("active");

    // `slice` here to remove the first `/` in pathname
    const currentPath = window.location.pathname.slice("1");

    // `link.href` returns a whole url, such as: "https://somedomain.com/posts" and we only need the last part
    const hrefArray = link.href.split("/");
    const thisPath = hrefArray[3];
    const currentPathArr = currentPath.split("/")

    if (currentPathArr[0] === thisPath) {
      link.classList.add("active");
    }
  });